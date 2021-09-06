using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Extensions
{
    internal sealed class CachedResource : IResource
    {
        private readonly AppContextCache<CacheData> resourceCache;
        private readonly AppFactory appFactory;
        private readonly IResourceGroup group;
        private readonly ResourceName name;
        private CacheData cacheData = new CacheData(new EntityID(), new ResourceName(""));

        public CachedResource(IMemoryCache cache, AppFactory appFactory, IResourceGroup group, ResourceName name)
        {
            this.appFactory = appFactory;
            this.group = group;
            this.name = name;
            resourceCache = new AppContextCache<CacheData>(cache, $"xti_resource_{group.ID.Value}_{name.Value}");
        }

        public EntityID ID { get => cacheData.ID; }

        public ResourceName Name() => cacheData.Name;

        public async Task Load()
        {
            cacheData = resourceCache.Get();
            if (cacheData == null)
            {
                var group = await appFactory.Groups().Group(this.group.ID.Value);
                var resource = await group.Resource(name);
                cacheData = new CacheData(resource.ID, resource.Name());
                resourceCache.Set(cacheData);
            }
        }

        private sealed record CacheData(EntityID ID, ResourceName Name);
    }
}
