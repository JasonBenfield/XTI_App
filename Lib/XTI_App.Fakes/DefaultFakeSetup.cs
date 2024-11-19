using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class DefaultFakeSetup : IAppSetup
{
    private readonly AppApiFactory apiFactory;
    private readonly FakeAppContext appContext;
    private AppContextModel? app;

    public DefaultFakeSetup(AppApiFactory apiFactory, FakeAppContext appContext)
    {
        this.apiFactory = apiFactory;
        this.appContext = appContext;
    }

    public AppContextModel App
    {
        get => app ?? throw new ArgumentNullException(nameof(app));
        private set => app = value;
    }

    public Task Run(AppVersionKey versionKey)
    {
        var template = apiFactory.CreateTemplate();
        var templateModel = template.ToModel();
        App = appContext.RegisterApp(templateModel);
        appContext.SetCurrentApp(App);
        return Task.CompletedTask;
    }
}