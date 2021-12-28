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
        var setup = new DefaultFakeSetup(apiFactory, appContext, options.Title);
        await setup.Run(versionKey);
        App = setup.App;
        var departmentModCategoryName = new ModifierCategoryName("Department");
        var departmentModCategory = await App.ModCategory(departmentModCategoryName);
        departmentModCategory.AddModifier(new ModifierKey("IT"));
        departmentModCategory.AddModifier(new ModifierKey("HR"));
        var userName = new AppUserName("xartogg");
        User = userContext.AddUser(userName);
        userContext.SetCurrentUser(userName);
    }
}