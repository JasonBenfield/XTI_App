using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeAppContext : ISourceAppContext
{
    private readonly List<FakeApp> apps = new List<FakeApp>();
    private FakeApp? currentApp;
    private FakeAppVersion? version;

    public FakeAppContext(AppKey? appKey)
    {
        var hubAppKey = AppKey.WebApp("Hub");
        var hubApp = AddNewApp(hubAppKey);
        var modCategory = hubApp.AddModCategory(new ModifierCategoryName("Apps"));
        modCategory.AddModifier
        (
            new ModifierKey(Guid.NewGuid().ToString("N")),
            hubApp.ID.ToString()
        );
        if (appKey != null)
        {
            if (appKey.Equals(hubAppKey))
            {
                SetCurrentApp(hubApp);
            }
            else
            {
                var app = AddApp(appKey);
                SetCurrentApp(app);
            }
        }
    }

    Task<IApp> IAppContext.App() => Task.FromResult<IApp>(App());

    public FakeApp App() =>
        currentApp ?? throw new ArgumentException("currentApp is null");

    public FakeApp AddApp(AppKey appKey)
    {
        var app = GetApp(appKey);
        if (app == null)
        {
            app = AddNewApp(appKey);
            var hubApp = App(AppKey.WebApp("Hub"));
            var modCategory = hubApp.ModCategory(new ModifierCategoryName("Apps"));
            modCategory.AddModifier
            (
                new ModifierKey(Guid.NewGuid().ToString("N")),
                app.ID.ToString()
            );
        }
        return app;
    }

    private FakeApp AddNewApp(AppKey appKey)
    {
        var id = FakeApp.NextID();
        var app = new FakeApp(this, id, appKey);
        version = app.CurrentVersion();
        apps.Add(app);
        return app;
    }

    public FakeApp App(AppKey appKey) =>
        GetApp(appKey)
        ?? throw new Exception($"App '{appKey.Name.DisplayText} {appKey.Type.DisplayText}' was not found");

    private FakeApp? GetApp(AppKey appKey) => apps.FirstOrDefault(a => a.AppKey.Equals(appKey));

    public void SetCurrentApp(FakeApp currentApp) => this.currentApp = currentApp;

    public Task<IAppVersion> Version()
    {
        if (version == null) { throw new ArgumentException("version is null"); }
        return Task.FromResult<IAppVersion>(version);
    }

    public void SetVersion(FakeAppVersion version) => this.version = version;

    Task<ModifierKey> ISourceAppContext.ModKeyInHubApps(IApp app) =>
        Task.FromResult(ModKeyInHubApps(app));

    public ModifierKey ModKeyInHubApps(IApp app)
    {
        var hubApp = AddApp(AppKey.WebApp("Hub"));
        var modCategory = hubApp.AddModCategory(new ModifierCategoryName("Apps"));
        return modCategory.ModifierByTargetID(app.ID.ToString()).ModKey();
    }
}