using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeAppContext : ISourceAppContext
{
    private readonly List<AppContextModel> apps = new();
    private AppContextModel? currentApp;

    private static int appID = 1;
    private static int resourceGroupID = 101;
    private static int modCategoryID = 201;
    private static int resourceID = 301;
    private static int roleID = 401;
    private static int modifierID = 501;

    public FakeAppContext(AppKey? appKey)
    {
        var hubAppKey = AppKey.WebApp("Hub");
        var hubApp = AddNewApp(new AppApiTemplateModel { AppKey = hubAppKey });
        if (appKey != null)
        {
            if (appKey.Equals(hubAppKey))
            {
                SetCurrentApp(hubApp);
            }
            else
            {
                var app = AddApp(new AppApiTemplateModel { AppKey = appKey });
                SetCurrentApp(app);
            }
        }
    }

    public Task<AppContextModel> App()
    {
        return Task.FromResult(currentApp ?? throw new ArgumentNullException(nameof(currentApp)));
    }

    public AppContextModel Update(AppContextModel original, Func<AppContextModel, AppContextModel> update)
    {
        apps.RemoveAll(a => a.App.AppKey.Equals(original.App.AppKey));
        var updated = update(original);
        apps.Add(updated);
        if (currentApp != null && currentApp.App.AppKey.Equals(updated.App.AppKey))
        {
            SetCurrentApp(updated);
        }
        return updated;
    }

    public AppContextModel AddApp(AppApiTemplateModel appTemplate)
    {
        apps.RemoveAll(a => a.App.AppKey.Equals(appTemplate.AppKey));
        var app = AddNewApp(appTemplate);
        if(currentApp != null && currentApp.App.AppKey.Equals(appTemplate.AppKey))
        {
            SetCurrentApp(app);
        }
        return app;
    }

    public AppContextModel AddModifier
    (
        AppContextModel appContext,
        ModifierCategoryName categoryName,
        ModifierKey modifierKey,
        string targetKey
    )
    {
        var modCategory = GetModifierCategory(appContext, categoryName);
        modCategory = modCategory with
        {
            Modifiers = modCategory.Modifiers
                .Union
                (
                    new[]
                    {
                        new ModifierModel(modifierID, modCategory.ModifierCategory.ID, modifierKey, targetKey, targetKey)
                    }
                )
                .ToArray()
        };
        modifierID++;
        var updated = Update
        (
            appContext,
            a => a with
            {
                ModifierCategories = a.ModifierCategories
                    .Where(mc => !mc.ModifierCategory.Name.Equals(categoryName))
                    .Union(new[] { modCategory })
                    .ToArray()
            }
        );
        return updated;
    }

    public AppContextModifierCategoryModel GetModifierCategory(AppContextModel appContext, ModifierCategoryName categoryName) =>
        appContext.ModifierCategory(categoryName);

    private AppContextModel AddNewApp(AppApiTemplateModel appTemplate)
    {
        var id = appID;
        var modCategoryNames = appTemplate.GroupTemplates.Select(g => g.ModCategory).ToArray();
        if (!modCategoryNames.Any(mc => ModifierCategoryName.Default.Equals(mc)))
        {
            modCategoryNames = modCategoryNames
                .Union(new[] { ModifierCategoryName.Default.DisplayText })
                .ToArray();
        }
        var modCategories = modCategoryNames
            .Distinct()
            .Select
            (
                mc =>
                {
                    var modCategory = new AppContextModifierCategoryModel
                    (
                        new ModifierCategoryModel
                        (
                            modCategoryID,
                            string.IsNullOrWhiteSpace(mc)
                                ? ModifierCategoryName.Default
                                : new ModifierCategoryName(mc)
                        ),
                        ModifierCategoryName.Default.Equals(mc)
                            ? new[] { new ModifierModel(modifierID, modCategoryID, ModifierKey.Default, "", "") }
                            : new ModifierModel[0]
                    );
                    modCategoryID++;
                    return modCategory;
                }
            )
            .ToArray();
        var roles = appTemplate.RecursiveRoles()
            .Union(AppRoleName.DefaultRoles().Select(r => r.DisplayText))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select
            (
                r =>
                {
                    var role = new AppRoleModel(roleID, new AppRoleName(r));
                    roleID++;
                    return role;
                }
            )
            .ToArray();
        var app = new AppContextModel
        (
            new AppModel
            (
                id,
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
                            resourceGroupID,
                            modCategories.First(mc => mc.ModifierCategory.Name.Equals(g.ModCategory)).ModifierCategory.ID,
                            new ResourceGroupName(g.Name),
                            g.IsAnonymousAllowed
                        ),
                        g.ActionTemplates.Select
                        (
                            a =>
                            {
                                var resource = new AppContextResourceModel
                                (
                                    new ResourceModel
                                    (
                                        resourceID,
                                        new ResourceName(a.Name),
                                        a.IsAnonymousAllowed,
                                        a.ResultType
                                    ),
                                    a.Roles
                                        .Select(r => roles.First(role => role.Name.Equals(r)))
                                        .ToArray()
                                );
                                resourceID++;
                                return resource;
                            }
                        )
                        .ToArray(),
                        g.Roles
                            .Select(r => roles.First(role => role.Name.Equals(r)))
                            .ToArray()
                    );
                    return group;
                }
            )
            .ToArray()
        );
        AddModifier(app, ModifierCategoryName.Default, ModifierKey.Default, "");
        appID++;
        return app;
    }

    public AppContextModel GetCurrentApp() => currentApp ?? throw new ArgumentNullException(nameof(currentApp));

    public void SetCurrentApp(AppContextModel currentApp) => this.currentApp = currentApp;
}