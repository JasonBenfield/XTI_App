using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions
{
    internal sealed class CachedAppVersion : IAppVersion
    {
        private readonly IMemoryCache cache;
        private readonly AppContextCache<CacheData> versionCache;
        private readonly IApp app;
        private readonly AppVersionKey versionKey;
        private readonly ISourceAppContext sourceAppContext;
        private CacheData cacheData = new CacheData(new EntityID(), AppVersionKey.None);

        public CachedAppVersion(IMemoryCache cache, ISourceAppContext sourceAppContext, IApp app, AppVersionKey versionKey)
        {
            this.cache = cache;
            this.sourceAppContext = sourceAppContext;
            this.app = app;
            this.versionKey = versionKey;
            versionCache = new AppContextCache<CacheData>(cache, $"xti_version_{app.ID.Value}_{versionKey.Value}");
        }

        public EntityID ID { get => cacheData.ID; }
        public AppVersionKey Key() => cacheData.Key;

        public async Task Load()
        {
            cacheData = versionCache.Get();
            if (cacheData == null)
            {
                var version = await getSourceVersion();
                cacheData = new CacheData(version.ID, version.Key());
                versionCache.Set(cacheData);
            }
        }

        private async Task<IAppVersion> getSourceVersion()
        {
            var sourceApp = await sourceAppContext.App();
            var version = await sourceApp.Version(versionKey);
            return version;
        }

        public async Task<IResourceGroup> ResourceGroup(ResourceGroupName name)
        {
            var cachedGroup = new CachedResourceGroup(cache, sourceAppContext, app, name);
            await cachedGroup.Load();
            return cachedGroup;
        }

        private sealed record CacheData(EntityID ID, AppVersionKey Key);

    }
}
