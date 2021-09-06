using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Extensions
{
    internal sealed class CachedAppRole : IAppRole
    {
        private readonly AppContextCache<CacheData> roleCache;
        private readonly AppFactory appFactory;
        private readonly int appID;
        private readonly AppRoleName name;
        private CacheData cacheData = new CacheData(new EntityID(), new AppRoleName("None"));

        public CachedAppRole(IMemoryCache cache, AppFactory appFactory, IApp app, AppRole role)
            : this(cache, appFactory, app, role.Name())
        {
            store(role);
        }

        public CachedAppRole(IMemoryCache cache, AppFactory appFactory, IApp app, AppRoleName name)
        {
            this.appFactory = appFactory;
            appID = app.ID.Value;
            this.name = name;
            roleCache = new AppContextCache<CacheData>(cache, $"xti_role_{appID}_{name.Value}");
        }

        public EntityID ID { get => cacheData.ID; }

        public AppRoleName Name() => cacheData.Name;

        public async Task Load()
        {
            cacheData = roleCache.Get();
            if (cacheData == null)
            {
                var app = await appFactory.Apps().App(appID);
                var role = await app.Role(name);
                store(role);
            }
        }

        private void store(AppRole role)
        {
            cacheData = new CacheData(role.ID, role.Name());
            roleCache.Set(cacheData);
        }

        private sealed record CacheData(EntityID ID, AppRoleName Name);
    }
}
