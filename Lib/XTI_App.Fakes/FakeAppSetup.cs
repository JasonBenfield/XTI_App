using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeAppSetup : IAppSetup
{
    private readonly FakeAppApiFactory apiFactory;
    private readonly FakeAppContext appContext;
    private readonly FakeUserContext userContext;
    private readonly FakeAppOptions options;

    private FakeApp? app;
    private FakeAppUser? user;

    public FakeAppSetup(FakeAppApiFactory apiFactory, FakeAppContext appContext, FakeUserContext userContext, FakeAppOptions options)
    {
        this.apiFactory = apiFactory;
        this.appContext = appContext;
        this.userContext = userContext;
        this.options = options;
    }

    public FakeApp App
    {
        get => app ?? throw new ArgumentNullException(nameof(app));
        private set => app = value;
    }

    public FakeAppUser User
    {
        get => user ?? throw new ArgumentNullException(nameof(user));
        private set => user = value;
    }

    public async Task Run(AppVersionKey versionKey)
    {
        var template = apiFactory.CreateTemplate();
        var templateModel = template.ToModel();
        App = appContext.AddApp(options.Title);
        appContext.SetCurrentApp(App);
        var modCategories = templateModel.GroupTemplates.Select(g => g.ModCategory).Distinct();
        foreach (var modCategory in modCategories)
        {
            App.AddModCategory(new ModifierCategoryName(modCategory));
        }
        var existingRoles = await App.Roles();
        var roles = templateModel.RecursiveRoles();
        foreach (var role in roles)
        {
            var roleName = new AppRoleName(role);
            if (!existingRoles.Any(r => r.Name().Equals(roleName)))
            {
                App.AddRole(roleName);
            }
        }
        var version = await App.Version(versionKey);
        if (version == null)
        {
            version = App.AddVersion(versionKey);
        }
        foreach (var groupTemplate in templateModel.GroupTemplates)
        {
            var group = version.AddResourceGroup(new ResourceGroupName(groupTemplate.Name), new ModifierCategoryName(groupTemplate.ModCategory));
            foreach (var actionTemplate in groupTemplate.ActionTemplates)
            {
                group.AddResource(new ResourceName(actionTemplate.Name));
            }
        }
        var departmentModCategoryName = new ModifierCategoryName("Department");
        var departmentModCategory = await App.ModCategory(departmentModCategoryName);
        departmentModCategory.AddModifier(new ModifierKey("IT"));
        departmentModCategory.AddModifier(new ModifierKey("HR"));
        var userName = new AppUserName("xartogg");
        User = userContext.AddUser(userName);
        userContext.SetCurrentUser(userName);
    }
}