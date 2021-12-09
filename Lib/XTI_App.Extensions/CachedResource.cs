using Microsoft.Extensions.Caching.Memory;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

internal sealed class CachedResource : IResource
{
    private readonly AppContextCache<CacheData> resourceCache;
    private readonly ISourceAppContext sourceAppContext;
    private readonly ResourceGroupName groupName;
    private readonly ResourceName name;
    private CacheData cacheData = new CacheData(new EntityID(), new ResourceName(""));

    public CachedResource(IMemoryCache cache, ISourceAppContext sourceAppContext, IResourceGroup group, ResourceName name)
    {
        this.sourceAppContext = sourceAppContext;
        groupName = group.Name();
        this.name = name;
        resourceCache = new AppContextCache<CacheData>(cache, $"xti_resource_{groupName.Value}_{name.Value}");
    }

    public EntityID ID { get => cacheData.ID; }

    public ResourceName Name() => cacheData.Name;

    public async Task Load()
    {
        cacheData = resourceCache.Get();
        if (cacheData == null)
        {
            var version = await sourceAppContext.Version();
            var group = await version.ResourceGroup(groupName);
            var resource = await group.Resource(name);
            cacheData = new CacheData(resource.ID, resource.Name());
            resourceCache.Set(cacheData);
        }
    }

    private sealed record CacheData(EntityID ID, ResourceName Name);
}