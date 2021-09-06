using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions
{
    internal sealed class CachedModifier : IModifier
    {
        private readonly AppContextCache<CacheData> modCache;
        private readonly ISourceAppContext sourceAppContext;
        private readonly IApp app;
        private readonly ModifierCategoryName modCategoryName;
        private readonly ModifierKey modKey;
        private CacheData cacheData = new CacheData(new EntityID(), ModifierKey.Default);

        public CachedModifier(IMemoryCache cache, ISourceAppContext sourceAppContext, IApp app, IModifierCategory modCategory, ModifierKey modKey)
        {
            this.sourceAppContext = sourceAppContext;
            this.app = app;
            modCategoryName = modCategory.Name();
            this.modKey = modKey;
            modCache = new AppContextCache<CacheData>(cache, $"xti_modifier_{modCategoryName.Value}_{modKey.Value}");
        }

        public EntityID ID { get => cacheData.ID; }

        public ModifierKey ModKey() => cacheData.ModKey;

        internal async Task Load()
        {
            cacheData = modCache.Get();
            if (cacheData == null)
            {
                var app = await sourceAppContext.App();
                var modCategory = await app.ModCategory(modCategoryName);
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
