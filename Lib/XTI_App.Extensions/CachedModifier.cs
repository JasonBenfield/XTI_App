using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using Microsoft.Extensions.DependencyInjection;

namespace XTI_App.Extensions
{
    internal sealed class CachedModifier : IModifier
    {
        private readonly IServiceProvider services;
        private readonly ModifierKey modKey;

        public CachedModifier(IServiceProvider services, IModifier source)
        {
            this.services = services;
            ID = source.ID;
            modKey = source.ModKey();
        }

        public EntityID ID { get; }

        public ModifierKey ModKey() => modKey;

        private CachedModifier cachedDefaultModifier;

        public async Task<IModifier> DefaultModifier()
        {
            if(cachedDefaultModifier == null)
            {
                var app = await appFromContext();
                var defaultModCat = await app.ModCategory(ModifierCategoryName.Default);
                var defaultModifier = await defaultModCat.Modifier(ModifierKey.Default);
                cachedDefaultModifier = new CachedModifier(services, defaultModifier);
            }
            return cachedDefaultModifier;
        }

        private Task<IApp> appFromContext()
        {
            var appContext = services.GetService<ISourceAppContext>();
            return appContext.App();
        }

    }
}
