using Microsoft.Extensions.Caching.Memory;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

internal sealed class CachedAppRole : IAppRole
{
    private readonly IMemoryCache cache;   
    private readonly AppContextCache<CacheData> roleCache;
    private readonly ISourceAppContext sourceAppContext;
    private CacheData cacheData = new CacheData(new EntityID(), new AppRoleName("None"));

    public CachedAppRole(IMemoryCache cache, ISourceAppContext sourceAppContext, IAppRole role)
        : this(cache, sourceAppContext, role.Name())
    {
        store(role);
    }

    public CachedAppRole(IMemoryCache cache, ISourceAppContext sourceAppContext, AppRoleName name)
    {
        this.cache = cache;
        this.sourceAppContext = sourceAppContext;
        roleCache = new AppContextCache<CacheData>(cache, $"xti_role_{name.Value}");
    }

    public EntityID ID { get => cacheData.ID; }

    public AppRoleName Name() => cacheData.Name;

    public async Task Load()
    {
        cacheData = roleCache.Get();
        if (cacheData == null)
        {
            var app = await sourceAppContext.App();
            var roles = await app.Roles();
            foreach(var role in roles)
            {
                new CachedAppRole(cache, sourceAppContext, role);
            }
        }
    }

    private void store(IAppRole role)
    {
        cacheData = new CacheData(role.ID, role.Name());
        roleCache.Set(cacheData);
    }

    private sealed record CacheData(EntityID ID, AppRoleName Name);
}