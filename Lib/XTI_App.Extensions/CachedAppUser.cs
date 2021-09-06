using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Extensions
{
    internal sealed class CachedAppUser : IAppUser
    {
        private readonly IMemoryCache cache;
        private readonly AppFactory appFactory;
        private readonly AppUserName userName;
        private readonly string cacheKey;
        private CacheData cacheData = new CacheData(new EntityID(), AppUserName.Anon);

        public CachedAppUser(IMemoryCache cache, AppFactory appFactory, AppUserName userName)
        {
            this.cache = cache;
            this.appFactory = appFactory;
            this.userName = userName;
            cacheKey = $"xti_user_{userName.Value}";
        }

        public EntityID ID { get => cacheData.ID; }
        public AppUserName UserName() => cacheData.UserName;

        internal void ClearCache()
        {
            cacheData = cache.Get<CacheData>(cacheKey);
            if (cacheData != null)
            {
                foreach (var subKey in cacheData.CacheKeys)
                {
                    cache.Remove(subKey);
                }
            }
            cache.Remove(cacheKey);
        }

        internal async Task Load()
        {
            cacheData = cache.Get<CacheData>(cacheKey);
            if (cacheData == null)
            {
                var user = await appFactory.Users().User(userName);
                cacheData = new CacheData(user.ID, user.UserName());
                store();
            }
        }

        private void store()
        {
            cache.Set
            (
                cacheKey,
                cacheData,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(4, 0, 0)
                }
            );
        }

        public async Task<IAppRole[]> Roles(IModifier modifier)
        {
            var cachedUserRoles = new List<CachedAppRole>();
            var rolesKey = $"xti_user_roles_{userName.Value}_{modifier.ID.Value}";
            var cachedRoleNames = cache.Get<AppRoleName[]>(rolesKey);
            var app = await modifier.App();
            if (cachedRoleNames == null)
            {
                cacheData.AddCacheKey(rolesKey);
                store();
                var user = await appFactory.Users().User(userName);
                var assignedRoles = await user.AssignedRoles(modifier);
                cachedRoleNames = assignedRoles.Select(r => r.Name()).ToArray();
                cache.Set
                (
                    rolesKey,
                    cachedRoleNames,
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = new TimeSpan(1, 0, 0)
                    }
                );
                foreach (var assignedRole in assignedRoles)
                {
                    var cachedRole = new CachedAppRole(cache, appFactory, app, assignedRole);
                    cachedUserRoles.Add(cachedRole);
                }
            }
            else
            {
                foreach (var roleName in cachedRoleNames)
                {
                    var cachedRole = new CachedAppRole(cache, appFactory, app, roleName);
                    await cachedRole.Load();
                    cachedUserRoles.Add(cachedRole);
                }
            }
            return cachedUserRoles.ToArray();
        }

        private sealed class CacheData
        {
            private readonly List<string> cacheKeys = new List<string>();

            public CacheData(EntityID id, AppUserName userName)
            {
                ID = id;
                UserName = userName;
            }

            public EntityID ID { get; }
            public AppUserName UserName { get; }
            public string[] CacheKeys { get => cacheKeys.ToArray(); }

            public void AddCacheKey(string cacheKey)
            {
                if (!cacheKeys.Contains(cacheKey))
                {
                    cacheKeys.Add(cacheKey);
                }
            }
        }
    }
}
