using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.EfApi
{
    public sealed class DefaultAppContext : ISourceAppContext
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
