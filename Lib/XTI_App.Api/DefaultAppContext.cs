using System.Threading.Tasks;

namespace XTI_App.Api
{
    public sealed class DefaultAppContext : IAppContext
    {
        private readonly AppFactory appFactory;
        private readonly AppKey appKey;
        private readonly AppVersionKey versionKey;

        public DefaultAppContext(AppFactory appFactory, AppKey appKey, AppVersionKey versionKey)
        {
            this.appFactory = appFactory;
            this.appKey = appKey;
            this.versionKey = versionKey;
        }

        public async Task<IApp> App() => await appFactory.Apps().App(appKey);

        public async Task<IAppVersion> Version()
        {
            var app = await App();
            var version = await app.Version(versionKey);
            return version;
        }
    }
}
