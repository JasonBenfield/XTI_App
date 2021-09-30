using System.Collections.Generic;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes
{
    public sealed class FakeAppContext : ISourceAppContext
    {
        private readonly List<FakeApp> apps = new List<FakeApp>();
        private FakeApp currentApp;
        private FakeAppVersion version;

        public FakeAppContext(string title = "")
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                var app = AddApp(title);
                SetCurrentApp(app);
            }
        }

        async Task<IApp> IAppContext.App() => await App();

        public Task<FakeApp> App() => Task.FromResult(currentApp);

        public FakeApp AddApp(string title)
        {
            var id = FakeApp.NextID();
            var app = new FakeApp(id, title);
            version = app.CurrentVersion();
            apps.Add(app);
            return app;
        }

        public void SetCurrentApp(FakeApp currentApp) => this.currentApp = currentApp;

        public Task<IAppVersion> Version() => Task.FromResult<IAppVersion>(version);

        public void SetVersion(FakeAppVersion version) => this.version = version;

    }
}
