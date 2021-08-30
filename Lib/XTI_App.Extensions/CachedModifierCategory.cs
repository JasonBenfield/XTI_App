using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions
{
    internal sealed class CachedModifierCategory : IModifierCategory
    {
        private readonly IServiceProvider services;
        private readonly ModifierCategoryName name;

        public CachedModifierCategory(IServiceProvider services, IModifierCategory modCategory)
        {
            this.services = services;
            ID = modCategory.ID;
            name = modCategory.Name();
        }

        public EntityID ID { get; }
        public ModifierCategoryName Name() => name;

        private readonly ConcurrentDictionary<string, CachedModifier> modifiers = new ConcurrentDictionary<string, CachedModifier>();

        public async Task<IModifier> Modifier(ModifierKey modKey)
        {
            if (!modifiers.TryGetValue(modKey.Value, out var cachedMod))
            {
                var app = await appFromContext();
                var modCategory = await app.ModCategory(name);
                var modifier = await modCategory.Modifier(modKey);
                cachedMod = new CachedModifier(services, modifier);
                modifiers.AddOrUpdate(modKey.Value, cachedMod, (key, mod) => cachedMod);
            }
            return cachedMod;
        }

        private Task<IApp> appFromContext()
        {
            var appContext = services.GetService<ISourceAppContext>();
            return appContext.App();
        }

    }
}
