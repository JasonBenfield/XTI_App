using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeAppContext : ISourceAppContext
{
    private readonly List<FakeApp> apps = new List<FakeApp>();
    private FakeApp? currentApp;
    private FakeAppVersion? version;

    public FakeAppContext(string title = "")
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            var app = AddApp(title);
            SetCurrentApp(app);
        }
    }

    Task<IApp> IAppContext.App() => Task.FromResult<IApp>(App());

    public FakeApp App() =>
        currentApp ?? throw new ArgumentException("currentApp is null");

    public FakeApp AddApp(string title)
    {
        var id = FakeApp.NextID();
        var app = new FakeApp(id, title);
        version = app.CurrentVersion();
        apps.Add(app);
        return app;
    }

    public void SetCurrentApp(FakeApp currentApp) => this.currentApp = currentApp;

    public Task<IAppVersion> Version()
    {
        if (version == null) { throw new ArgumentException("version is null"); }
        return Task.FromResult<IAppVersion>(version);
    }

    public void SetVersion(FakeAppVersion version) => this.version = version;
}