using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Extensions
{
    internal sealed class CachedResourceGroup : IResourceGroup
    {
        private readonly IMemoryCache cache;
        private readonly AppContextCache<CacheData> groupCache;
        private readonly AppFactory appFactory;
        private readonly IApp app;
        private readonly IAppVersion version;
        private readonly ResourceGroupName groupName;
        private CacheData cacheData;

        public CachedResourceGroup(IMemoryCache cache, AppFactory appFactory, IApp app, IAppVersion version, ResourceGroupName groupName)
        {
            this.cache = cache;
            this.appFactory = appFactory;
            this.app = app;
            this.version = version;
            this.groupName = groupName;
            groupCache = new AppContextCache<CacheData>(cache, $"xti_resource_group_{version.ID.Value}_{groupName.Value}");
        }

        public EntityID ID { get => cacheData?.ID ?? new EntityID(); }
        public ResourceGroupName Name() => cacheData?.Name ?? new ResourceGroupName("");

        public async Task Load()
        {
            cacheData = groupCache.Get();
            if (cacheData == null)
            {
                var version = await appFactory.Versions().Version(this.version.ID.Value);
                var group = await version.ResourceGroup(groupName);
                var modCategory = await group.ModCategory();
                new CachedModifierCategory(cache, appFactory, app, modCategory);
                cacheData = new CacheData(group.ID, group.Name(), modCategory.Name());
                groupCache.Set(cacheData);
            }
        }

        public async Task<IModifierCategory> ModCategory()
        {
            var cachedModCategory = new CachedModifierCategory(cache, appFactory, app, cacheData.ModCategoryName);
            await cachedModCategory.Load();
            return cachedModCategory;
        }

        public async Task<IResource> Resource(ResourceName name)
        {
            var cachedResource = new CachedResource(cache, appFactory, this, name);
            await cachedResource.Load();
            return cachedResource;
        }

        private sealed record CacheData(EntityID ID, ResourceGroupName Name, ModifierCategoryName ModCategoryName);
    }
}
