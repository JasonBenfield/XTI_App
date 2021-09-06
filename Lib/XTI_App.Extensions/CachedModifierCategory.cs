using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Extensions
{
    internal sealed class CachedModifierCategory : IModifierCategory
    {
        private readonly IMemoryCache cache;
        private readonly AppContextCache<CacheData> modCategoryCache;
        private readonly AppFactory appFactory;
        private readonly IApp parentApp;
        private readonly ModifierCategoryName name;
        private CacheData cacheData = new CacheData(new EntityID(), new ModifierCategoryName(""));

        public CachedModifierCategory(IMemoryCache cache, AppFactory appFactory, IApp parentApp, ModifierCategory modCategory)
            : this(cache, appFactory, parentApp, modCategory.Name())
        {
            store(modCategory);
        }

        public CachedModifierCategory(IMemoryCache cache, AppFactory appFactory, IApp parentApp, ModifierCategoryName name)
        {
            this.cache = cache;
            this.appFactory = appFactory;
            this.parentApp = parentApp;
            this.name = name;
            modCategoryCache = new AppContextCache<CacheData>(cache, $"xti_mod_category_{parentApp.ID.Value}_{name.Value}");
        }

        public EntityID ID { get => cacheData?.ID ?? new EntityID(); }
        public ModifierCategoryName Name() => cacheData?.Name ?? new ModifierCategoryName("");

        internal async Task Load()
        {
            cacheData = modCategoryCache.Get();
            if (cacheData == null)
            {
                var app = await appFactory.Apps().App(parentApp.ID.Value);
                var modCategory = await app.ModCategory(name);
                store(modCategory);
            }
        }

        private void store(ModifierCategory modCategory)
        {
            cacheData = new CacheData(modCategory.ID, modCategory.Name());
            modCategoryCache.Set(cacheData);
        }

        public async Task<IModifier> Modifier(ModifierKey modKey)
        {
            var cachedMod = new CachedModifier(cache, appFactory, parentApp, this, modKey);
            await cachedMod.Load();
            return cachedMod;
        }

        private sealed record CacheData(EntityID ID, ModifierCategoryName Name);

    }
}
