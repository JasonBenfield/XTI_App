using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class DefaultFakeSetup : IAppSetup
{
    private readonly AppApiFactory apiFactory;
    private readonly FakeAppContext appContext;
    private readonly string title;
    private FakeApp? app;

    public DefaultFakeSetup(AppApiFactory apiFactory, FakeAppContext appContext, string title)
    {
        this.apiFactory = apiFactory;
        this.appContext = appContext;
        this.title = title;
    }

    public FakeApp App
    {
        get => app ?? throw new ArgumentNullException(nameof(app));
        private set => app = value;
    }

    public Task Run(AppVersionKey versionKey)
    {
        var template = apiFactory.CreateTemplate();
        var templateModel = template.ToModel();
        App = appContext.AddApp(title);
        appContext.SetCurrentApp(App);
        var modCategories = templateModel.GroupTemplates.Select(g => g.ModCategory).Distinct();
        foreach (var modCategory in modCategories)
        {
            App.AddModCategory(new ModifierCategoryName(modCategory));
        }
        var existingRoles = App.Roles();
        var roles = templateModel.RecursiveRoles();
        foreach (var role in roles)
        {
            var roleName = new AppRoleName(role);
            App.AddRole(roleName);
        }
        var version = App.AddVersion(versionKey);
        foreach (var groupTemplate in templateModel.GroupTemplates)
        {
            var group = version.AddResourceGroup
            (
                new ResourceGroupName(groupTemplate.Name),
                new ModifierCategoryName(groupTemplate.ModCategory)
            );
            foreach (var actionTemplate in groupTemplate.ActionTemplates)
            {
                group.AddResource(new ResourceName(actionTemplate.Name));
            }
        }
        return Task.CompletedTask;
    }
}