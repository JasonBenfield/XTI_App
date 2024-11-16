using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeAppContext : ISourceAppContext
{
    private readonly List<ModifierModel> modifiers = new();
    private readonly AppKey appKey;
    private readonly int currentAppID;
    private AppContextModel currentApp;

    public FakeAppContext(AppKey appKey)
    {
        this.appKey = appKey;
        currentAppID = FakeAppDB.GetAppID(appKey);
        currentApp = RegisterApp(new AppApiTemplateModel { AppKey = appKey });
    }

    public Task<AppContextModel> App()
    {
        return Task.FromResult(currentApp ?? throw new ArgumentNullException(nameof(currentApp)));
    }

    public AppContextModel Update(AppContextModel original, Func<AppContextModel, AppContextModel> update)
    {
        currentApp = update(original);
        return currentApp;
    }

    public AppContextModel RegisterApp(AppApiTemplateModel appTemplate)
    {
        if (!appKey.Equals(appTemplate.AppKey))
        {
            throw new Exception($"App Context '{appKey.Format()}' can not register template '{appTemplate.AppKey.Format()}'");
        }
        var modCategoryNames = appTemplate.GroupTemplates.Select(g => g.ModCategory).ToArray();
        if (!modCategoryNames.Any(mc => ModifierCategoryName.Default.Equals(mc)))
        {
            modCategoryNames = modCategoryNames
                .Union([ModifierCategoryName.Default])
                .ToArray();
        }
        var modCategories = modCategoryNames
            .Distinct()
            .Select
            (
                mc => new ModifierCategoryModel
                (
                    FakeAppDB.GenerateModCategoryID(),
                    string.IsNullOrWhiteSpace(mc)
                        ? ModifierCategoryName.Default
                        : mc
                )
            )
            .ToArray();
        var roles = appTemplate.RecursiveRoles()
            .Union(AppRoleName.DefaultRoles())
            .Distinct()
            .Select
            (
                r => new AppRoleModel(FakeAppDB.GenerateRoleID(), r)
            )
            .ToArray();
        var defaultModCategory = modCategories.First(mc => mc.Name.Equals(ModifierCategoryName.Default));
        var defaultModifier = new ModifierModel(FakeAppDB.GenerateModifierID(), defaultModCategory.ID, ModifierKey.Default, "", "");
        modifiers.Add(defaultModifier);
        currentApp = new AppContextModel
        (
            new AppModel
            (
                currentAppID,
                appTemplate.AppKey,
                new AppVersionName("Fake"),
                new ModifierKey(appTemplate.AppKey.Format())
            ),
            new XtiVersionModel
            (
                1,
                new AppVersionName("Fake"),
                new AppVersionKey(1),
                new AppVersionNumber(1, 0, 0),
                AppVersionType.Values.Major,
                AppVersionStatus.Values.Current,
                DateTimeOffset.Now
            ),
            roles,
            modCategories,
            appTemplate.GroupTemplates.Select
            (
                g =>
                {
                    var group = new AppContextResourceGroupModel
                    (
                        new ResourceGroupModel
                        (
                            FakeAppDB.GenerateResourceGroupID(),
                            modCategories.First(mc => mc.Name.Equals(g.ModCategory)).ID,
                            g.Name,
                            g.IsAnonymousAllowed
                        ),
                        g.ActionTemplates.Select
                        (
                            a =>  new AppContextResourceModel
                            (
                                new ResourceModel
                                (
                                    FakeAppDB.GenerateResourceID(),
                                    a.Name,
                                    a.IsAnonymousAllowed,
                                    a.ResultType
                                ),
                                a.Roles
                                    .Select(r => roles.First(role => role.Name.Equals(r)))
                                    .ToArray()
                            )
                        )
                        .ToArray(),
                        g.Roles
                            .Select(r => roles.First(role => role.Name.Equals(r)))
                            .ToArray()
                    );
                    return group;
                }
            )
            .ToArray(),
            defaultModifier
        );
        return currentApp;
    }

    public AppContextModel AddModifier
    (
        ModifierCategoryName categoryName,
        ModifierKey modifierKey,
        string targetKey
    ) => AddModifier(GetCurrentApp(), categoryName, modifierKey, targetKey);

    private AppContextModel AddModifier
    (
        AppContextModel appContext,
        ModifierCategoryName categoryName,
        ModifierKey modifierKey,
        string targetKey
    )
    {
        var modCategory = GetModCategory(appContext, categoryName);
        _AddModifier(modCategory, modifierKey, targetKey);
        return GetCurrentApp();
    }

    public AppContextModel AddModifier
    (
        ModifierCategoryModel modCategory,
        ModifierKey modifierKey,
        string targetKey
    )
    {
        _AddModifier(modCategory, modifierKey, targetKey);
        return GetCurrentApp();
    }

    private void _AddModifier
    (
        ModifierCategoryModel modCategory,
        ModifierKey modifierKey,
        string targetKey
    )
    {
        var modifier = modifiers.FirstOrDefault(m => m.ModKey.Equals(modifierKey));
        if (modifier != null)
        {
            modifiers.Remove(modifier);
        }
        modifiers.Add
        (
            new ModifierModel
            (
                FakeAppDB.GenerateModifierID(),
                modCategory.ID,
                modifierKey,
                targetKey,
                targetKey
            )
        );
    }

    public ModifierCategoryModel GetModCategory(ModifierCategoryName categoryName) =>
        GetModCategory(GetCurrentApp(), categoryName);

    private ModifierCategoryModel GetModCategory(AppContextModel appContext, ModifierCategoryName categoryName) =>
        appContext.ModCategory(categoryName);

    public AppContextModel GetCurrentApp() => currentApp ?? throw new ArgumentNullException(nameof(currentApp));

    public void SetCurrentApp(AppContextModel currentApp) => this.currentApp = currentApp;

    public Task<ModifierModel> Modifier(ModifierCategoryModel category, ModifierKey modKey)
    {
        var modifier = GetModifier(category, modKey);
        return Task.FromResult(modifier);
    }

    public ModifierModel GetDefaultModifier() =>
        GetModifier(GetCurrentApp().ModCategory(ModifierCategoryName.Default), ModifierKey.Default);

    public ModifierModel GetModifier(ModifierCategoryModel category, ModifierKey modKey)
    {
        if (modKey.Equals(ModifierKey.Default))
        {
            category = GetModCategory(ModifierCategoryName.Default);
        }
        return modifiers.FirstOrDefault(m => m.CategoryID == category.ID && m.ModKey.Equals(modKey)) ?? new();
    }

    public AppRoleModel[] GetRoles(params AppRoleName[] roleNames) =>
        GetCurrentApp().Roles.Where(r => roleNames.Contains(r.Name)).ToArray();
}