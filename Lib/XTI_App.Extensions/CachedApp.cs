using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Extensions
{
    internal sealed class CachedApp : IApp
    {
        private readonly IMemoryCache cache;
        private readonly AppContextCache<CacheData> appCache;
        private readonly AppFactory appFactory;
        private readonly AppKey appKey;
        private CacheData cacheData = new CacheData(new EntityID(), "");

        public CachedApp(IMemoryCache cache, AppFactory appFactory, AppKey appKey)
        {
            this.cache = cache;
            this.appFactory = appFactory;
            this.appKey = appKey;
            appCache = new AppContextCache<CacheData>(cache, $"xti_{appKey.Type.Value}_{appKey.Name.Value}");
        }

        internal async Task Load()
        {
            cacheData = appCache.Get();
            if (cacheData == null)
            {
                var app = await appFactory.Apps().App(appKey);
                cacheData = new CacheData(app.ID, app.Title);
                appCache.Set(cacheData);
            }
        }

        public EntityID ID { get => cacheData.ID; }
        public string Title { get => cacheData.Title; }

        public Task<IAppVersion> CurrentVersion() => Version(AppVersionKey.Current);

        public async Task<IAppVersion> Version(AppVersionKey versionKey)
        {
            var cachedVersion = new CachedAppVersion(cache, appFactory, this, versionKey);
            await cachedVersion.Load();
            return cachedVersion;
        }

        public async Task<IAppRole[]> Roles()
        {
            var cachedRoles = new List<IAppRole>();
            var rolesKey = $"xti_roles_{appKey.Name.Value}";
            var cachedRoleNames = cache.Get<AppRoleName[]>(rolesKey);
            if (cachedRoleNames == null)
            {
                var app = await appFactory.Apps().App(appKey);
                var roles = await app.Roles();
                cachedRoleNames = roles.Select(r => r.Name()).ToArray();
                cache.Set
                (
                    rolesKey,
                    cachedRoleNames,
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = new TimeSpan(4, 0, 0)
                    }
                );
                foreach (var role in roles)
                {
                    var cachedRole = new CachedAppRole(cache, appFactory, this, role);
                    cachedRoles.Add(cachedRole);
                }
            }
            else
            {
                foreach (var roleName in cachedRoleNames)
                {
                    var cachedRole = new CachedAppRole(cache, appFactory, this, roleName);
                    await cachedRole.Load();
                    cachedRoles.Add(cachedRole);
                }
            }
            return cachedRoles.ToArray();
        }

        public async Task<IModifierCategory> ModCategory(ModifierCategoryName name)
        {
            var cachedModCategory = new CachedModifierCategory(cache, appFactory, this, name);
            await cachedModCategory.Load();
            return cachedModCategory;
        }

        private sealed record CacheData(EntityID ID, string Title);

    }
}
