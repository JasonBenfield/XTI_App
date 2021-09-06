using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Extensions
{
    internal sealed class CachedModifier : IModifier
    {
        private readonly AppContextCache<CacheData> modCache;
        private readonly AppFactory appFactory;
        private readonly IApp app;
        private readonly int modCategoryID;
        private readonly ModifierKey modKey;
        private CacheData cacheData = new CacheData(new EntityID(), ModifierKey.Default);

        public CachedModifier(IMemoryCache cache, AppFactory appFactory, IApp app, IModifierCategory modCategory, ModifierKey modKey)
        {
            this.appFactory = appFactory;
            this.app = app;
            modCategoryID = modCategory.ID.Value;
            this.modKey = modKey;
            modCache = new AppContextCache<CacheData>(cache, $"xti_modifier_{modCategory.ID.Value}_{modKey.Value}");
        }

        public EntityID ID { get => cacheData.ID; }

        public ModifierKey ModKey() => cacheData.ModKey;

        internal async Task Load()
        {
            cacheData = modCache.Get();
            if (cacheData == null)
            {
                var modCategory = await appFactory.ModCategories().Category(modCategoryID);
                var modifier = await modCategory.Modifier(modKey);
                cacheData = new CacheData(modifier.ID, modifier.ModKey());
                modCache.Set(cacheData);
            }
        }

        public async Task<IModifier> DefaultModifier()
        {
            var defaultModCategory = await app.ModCategory(ModifierCategoryName.Default);
            var defaultMod = await defaultModCategory.Modifier(ModifierKey.Default);
            return defaultMod;
        }

        public Task<IApp> App() => Task.FromResult(app);

        private sealed record CacheData(EntityID ID, ModifierKey ModKey);

    }
}
