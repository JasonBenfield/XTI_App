using Microsoft.Extensions.Caching.Memory;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

internal sealed class CachedResourceGroup : IResourceGroup
{
    private readonly IMemoryCache cache;
    private readonly AppContextCache<CacheData> groupCache;
    private readonly ISourceAppContext sourceAppContext;
    private readonly IApp app;
    private readonly ResourceGroupName groupName;
    private CacheData? cacheData;

    public CachedResourceGroup(IMemoryCache cache, ISourceAppContext sourceAppContext, IApp app, ResourceGroupName groupName)
    {
        this.cache = cache;
        this.sourceAppContext = sourceAppContext;
        this.app = app;
        this.groupName = groupName;
        groupCache = new AppContextCache<CacheData>(cache, $"xti_resource_group_{groupName.Value}");
    }

    public EntityID ID { get => cacheData?.ID ?? new EntityID(); }
    public ResourceGroupName Name() => cacheData?.Name ?? new ResourceGroupName("");

    public async Task Load()
    {
        cacheData = groupCache.Get();
        if (cacheData == null)
        {
            var version = await sourceAppContext.Version();
            var group = await version.ResourceGroup(groupName);
            var modCategory = await group.ModCategory();
            new CachedModifierCategory(cache, sourceAppContext, app, modCategory);
            cacheData = new CacheData(group.ID, group.Name(), modCategory.Name());
            groupCache.Set(cacheData);
        }
    }

    public async Task<IModifierCategory> ModCategory()
    {
        var cachedModCategory = new CachedModifierCategory
        (
            cache, 
            sourceAppContext, 
            app, 
            cacheData?.ModCategoryName ?? ModifierCategoryName.Default
        );
        await cachedModCategory.Load();
        return cachedModCategory;
    }

    public async Task<IResource> Resource(ResourceName name)
    {
        var cachedResource = new CachedResource(cache, sourceAppContext, this, name);
        await cachedResource.Load();
        return cachedResource;
    }

    private sealed record CacheData(EntityID ID, ResourceGroupName Name, ModifierCategoryName ModCategoryName);
}