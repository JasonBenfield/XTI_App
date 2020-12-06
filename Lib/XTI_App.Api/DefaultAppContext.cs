using System.Threading.Tasks;

namespace XTI_App.Api
{
    public sealed class DefaultAppContext : IAppContext
    {
        private readonly AppFactory appFactory;
        private readonly AppKey appKey;

        public DefaultAppContext(AppFactory appFactory, AppKey appKey)
        {
            this.appFactory = appFactory;
            this.appKey = appKey;
        }

        public async Task<IApp> App() => await appFactory.Apps().App(appKey);
    }
}
