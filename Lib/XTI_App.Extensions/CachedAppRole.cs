using Microsoft.Extensions.Caching.Memory;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

internal sealed class CachedAppRole : IAppRole
{
    private readonly AppContextCache<CacheData> roleCache;
    private readonly ISourceAppContext sourceAppContext;
    private readonly AppRoleName name;
    private CacheData cacheData = new CacheData(new EntityID(), new AppRoleName("None"));

    public CachedAppRole(IMemoryCache cache, ISourceAppContext sourceAppContext, IAppRole role)
        : this(cache, sourceAppContext, role.Name())
    {
        store(role);
    }

    public CachedAppRole(IMemoryCache cache, ISourceAppContext sourceAppContext, AppRoleName name)
    {
        this.sourceAppContext = sourceAppContext;
        this.name = name;
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
            var role = await app.Role(name);
            store(role);
        }
    }

    private void store(IAppRole role)
    {
        cacheData = new CacheData(role.ID, role.Name());
        roleCache.Set(cacheData);
    }

    private sealed record CacheData(EntityID ID, AppRoleName Name);
}