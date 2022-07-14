using XTI_App.Abstractions;
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
        apps.Remove(original);
        var updated = update(original);
        apps.Add(updated);
        return updated;
    }

    public AppContextModel AddApp(AppApiTemplateModel appTemplate)
    {
        apps.RemoveAll(a => a.App.AppKey.Equals(appTemplate.AppKey));
        var app = AddNewApp(appTemplate);
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
        var modCategory = appContext.ModCategory(categoryName);
        modCategory = modCategory with
        {
            Modifiers = modCategory.Modifiers
                .Union
                (
                    new[]
                    {
                        new ModifierModel(modifierID, modCategory.ID, modifierKey, targetKey, targetKey)
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
                ResourceGroups = appContext.ResourceGroups
                    .Select
                    (
                        rg => new AppContextResourceGroupModel
                        (
                            rg.ID,
                            rg.Name,
                            rg.IsAnonymousAllowed,
                            rg.ModifierCategory.Equals(categoryName)
                                ? modCategory
                                : rg.ModifierCategory,
                            rg.Resources
                        )
                    )
                    .ToArray()
            }
        );
        return updated;
    }

    private AppContextModel AddNewApp(AppApiTemplateModel appTemplate)
    {
        var id = appID;
        var modCategories = appTemplate.GroupTemplates
            .Select(g => g.ModCategory)
            .Distinct()
            .Select
            (
                mc =>
                {
                    var modCategory = new AppContextModifierCategoryModel
                    (
                        modCategoryID,
                        new ModifierCategoryName(mc),
                        new ModifierModel[0]
                    );
                    modCategoryID++;
                    return modCategory;
                }
            );
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
                appTemplate.AppKey.Name.DisplayText
            ),
            new ModifierKey(appTemplate.AppKey.Serialize()),
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
            appTemplate.GroupTemplates.Select
            (
                g =>
                {
                    var group = new AppContextResourceGroupModel
                    (
                        resourceGroupID,
                        new ResourceGroupName(g.Name),
                        g.IsAnonymousAllowed,
                        modCategories.First(mc => mc.Name.Equals(g.ModCategory)),
                        g.ActionTemplates.Select
                        (
                            a =>
                            {
                                var resource = new AppContextResourceModel
                                (
                                    resourceID,
                                    new ResourceName(a.Name),
                                    a.IsAnonymousAllowed,
                                    a.ResultType,
                                    a.Roles
                                        .Select(r => roles.First(role => role.Name.Equals(r)))
                                        .ToArray()
                                );
                                resourceID++;
                                return resource;
                            }
                        )
                        .ToArray()
                    );
                    return group;
                }
            )
            .ToArray()
        );
        appID++;
        return app;
    }

    public AppContextModel GetCurrentApp() => currentApp ?? throw new ArgumentNullException(nameof(currentApp));

    public void SetCurrentApp(AppContextModel currentApp) => this.currentApp = currentApp;
}