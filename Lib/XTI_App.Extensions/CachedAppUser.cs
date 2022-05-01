using Microsoft.Extensions.Caching.Memory;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

internal sealed class CachedAppUser : IAppUser
{
    private readonly IMemoryCache cache;
    private readonly ISourceAppContext sourceAppContext;
    private readonly ISourceUserContext sourceUserContext;
    private readonly AppUserName userName;
    private readonly string cacheKey;
    private CacheData cacheData = new CacheData(0, AppUserName.Anon);

    public CachedAppUser(IMemoryCache cache, ISourceAppContext sourceAppContext, ISourceUserContext sourceUserContext, AppUserName userName)
    {
        this.cache = cache;
        this.sourceAppContext = sourceAppContext;
        this.sourceUserContext = sourceUserContext;
        this.userName = userName;
        cacheKey = $"xti_user_{userName.Value}";
    }

    public int ID { get => cacheData.ID; }
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
            var user = await sourceUserContext.User(userName);
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
        var rolesKey = $"xti_user_{userName.Value}_roles_{modifier.ID}";
        var cachedRoleNames = cache.Get<AppRoleName[]>(rolesKey);
        if (cachedRoleNames == null)
        {
            cacheData.AddCacheKey(rolesKey);
            store();
            var user = await sourceUserContext.User();
            var assignedRoles = await user.Roles(modifier);
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
                var cachedRole = new CachedAppRole(cache, sourceAppContext, assignedRole);
                cachedUserRoles.Add(cachedRole);
            }
        }
        else
        {
            foreach (var roleName in cachedRoleNames)
            {
                var cachedRole = new CachedAppRole(cache, sourceAppContext, roleName);
                await cachedRole.Load();
                cachedUserRoles.Add(cachedRole);
            }
        }
        return cachedUserRoles.ToArray();
    }

    private sealed class CacheData
    {
        private readonly List<string> cacheKeys = new List<string>();

        public CacheData(int id, AppUserName userName)
        {
            ID = id;
            UserName = userName;
        }

        public int ID { get; }
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