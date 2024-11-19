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
        var cacheKey = $"xti_app_{appKey.Type.Value}_{appKey.Name.Value}";
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

    public async Task<ModifierModel> Modifier(ModifierCategoryModel category, ModifierKey modKey)
    {
        var cacheKey = $"xti_modifier_{appKey.Type.Value}_{appKey.Name.Value}_{category.Name.DisplayText}_{modKey.DisplayText}";
        if (!cache.TryGetValue<ModifierModel>(cacheKey, out var cachedModifier))
        {
            cachedModifier = await sourceAppContext.Modifier(category, modKey);
            cache.Set
            (
                cacheKey,
                cachedModifier,
                TimeSpan.FromHours(1)
            );
        }
        return cachedModifier ?? new ModifierModel();
    }
}