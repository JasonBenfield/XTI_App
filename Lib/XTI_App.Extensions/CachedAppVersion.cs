using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions
{
    internal sealed class CachedAppVersion : IAppVersion
    {
        private readonly IServiceProvider services;
        private readonly AppVersionKey key;

        public CachedAppVersion(IServiceProvider services, IAppVersion appVersion)
        {
            this.services = services;
            ID = appVersion.ID;
            key = appVersion.Key();
        }

        public EntityID ID { get; }
        public AppVersionKey Key() => key;

        private readonly ConcurrentDictionary<string, CachedResourceGroup> resourceGroupLookup
            = new ConcurrentDictionary<string, CachedResourceGroup>();

        public async Task<IResourceGroup> ResourceGroup(ResourceGroupName name)
        {
            if (!resourceGroupLookup.TryGetValue(name.Value, out var cachedResourceGroup))
            {
                var app = await appFromContext();
                var version = await app.Version(key);
                var group = await version.ResourceGroup(name);
                cachedResourceGroup = new CachedResourceGroup(services, group);
                resourceGroupLookup.AddOrUpdate(name.Value, cachedResourceGroup, (key, rg) => cachedResourceGroup);
            }
            return cachedResourceGroup;
        }

        private Task<IApp> appFromContext()
        {
            var appContext = services.GetService<ISourceAppContext>();
            return appContext.App();
        }

    }
}
