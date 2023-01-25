using Microsoft.Extensions.Caching.Memory;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

public sealed class CachedAppContext : IAppContext
{
    private readonly IMemoryCache cache;
    private readonly ISourceAppContext sourceAppContext;
    private readonly AppKey appKey;

    public CachedAppContext(IMemoryCache cache, ISourceAppContext sourceAppContext, AppKey appKey)
    {
        this.cache = cache;
        this.sourceAppContext = sourceAppContext;
        this.appKey = appKey;
    }

    public async Task<AppContextModel> App()
    {
        var cacheKey = $"xti_{appKey.Type.Value}_{appKey.Name.Value}";
        if(!cache.TryGetValue<AppContextModel>(cacheKey, out var cachedApp))
        {
            cachedApp = await sourceAppContext.App();
            cache.Set
            (
                cacheKey, 
                cachedApp,
                TimeSpan.FromHours(4)
            );
        }
        return cachedApp ?? new AppContextModel();
    }
}