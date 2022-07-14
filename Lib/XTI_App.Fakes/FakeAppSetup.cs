using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeAppSetup : IAppSetup
{
    private readonly FakeAppApiFactory apiFactory;
    private readonly FakeAppContext appContext;
    private readonly FakeUserContext userContext;

    private AppContextModel? app;
    private UserContextModel? user;

    public FakeAppSetup(FakeAppApiFactory apiFactory, FakeAppContext appContext, FakeUserContext userContext)
    {
        this.apiFactory = apiFactory;
        this.appContext = appContext;
        this.userContext = userContext;
    }

    public AppContextModel App
    {
        get => app ?? throw new ArgumentNullException(nameof(app));
        private set => app = value;
    }

    public UserContextModel User
    {
        get => user ?? throw new ArgumentNullException(nameof(user));
        private set => user = value;
    }

    public async Task Run(AppVersionKey versionKey)
    {
        var setup = new DefaultFakeSetup(apiFactory, appContext);
        await setup.Run(versionKey);
        App = setup.App;
        var departmentModCategoryName = new ModifierCategoryName("Department");
        App = appContext.AddModifier(App, departmentModCategoryName,new ModifierKey("IT"), "IT");
        App = appContext.AddModifier(App, departmentModCategoryName, new ModifierKey("HR"), "HR");
        var userName = new AppUserName("xartogg");
        User = userContext.AddUser(userName);
        userContext.SetCurrentUser(userName);
    }
}