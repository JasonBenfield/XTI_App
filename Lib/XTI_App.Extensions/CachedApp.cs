using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions
{
    internal sealed class CachedApp : IApp
    {
        private readonly IServiceProvider services;

        public CachedApp(IServiceProvider services, IApp app)
        {
            this.services = services;
            ID = app.ID;
            Title = app.Title;
        }

        public EntityID ID { get; }
        public string Title { get; }

        public Task<IAppVersion> CurrentVersion() => Version(AppVersionKey.Current);

        private readonly ConcurrentDictionary<string, CachedAppVersion> cachedVersionLookup = new ConcurrentDictionary<string, CachedAppVersion>();

        public async Task<IAppVersion> Version(AppVersionKey versionKey)
        {
            if (!cachedVersionLookup.TryGetValue(versionKey.Value, out var cachedVersion))
            {
                var app = await appFromContext();
                var version = await app.Version(versionKey);
                cachedVersion = new CachedAppVersion(services, version);
                cachedVersionLookup.AddOrUpdate(versionKey.Value, cachedVersion, (key, v) => cachedVersion);
            }
            return cachedVersion;
        }

        private IAppRole[] cachedAppRoles;

        public async Task<IAppRole[]> Roles()
        {
            if (cachedAppRoles == null)
            {
                var app = await appFromContext();
                var appRoles = await app.Roles();
                cachedAppRoles = appRoles.Select(r => new CachedAppRole(r)).ToArray();
            }
            return cachedAppRoles;
        }

        private readonly ConcurrentDictionary<string, CachedResourceGroup> resourceGroupLookup
            = new ConcurrentDictionary<string, CachedResourceGroup>();

        public async Task<IResourceGroup> ResourceGroup(ResourceGroupName name)
        {
            if (!resourceGroupLookup.TryGetValue(name.Value, out var cachedResourceGroup))
            {
                var app = await appFromContext();
                var version = await app.Version(AppVersionKey.Current);
                var group = await version.ResourceGroup(name);
                cachedResourceGroup = new CachedResourceGroup(services, group);
                resourceGroupLookup.AddOrUpdate(name.Value, cachedResourceGroup, (key, rg) => cachedResourceGroup);
            }
            return cachedResourceGroup;
        }

        private readonly ConcurrentDictionary<string, CachedModifierCategory> cachedModCategories = new ConcurrentDictionary<string, CachedModifierCategory>();

        public async Task<IModifierCategory> ModCategory(ModifierCategoryName name)
        {
            if (!cachedModCategories.TryGetValue(name.Value, out var cachedModCategory))
            {
                var app = await appFromContext();
                var modCategory = await app.ModCategory(name);
                cachedModCategory = new CachedModifierCategory(services, modCategory);
                cachedModCategories.AddOrUpdate(name.Value, cachedModCategory, (key, modCat) => cachedModCategory);
            }
            return cachedModCategory;
        }

        private Task<IApp> appFromContext()
        {
            var appContext = services.GetService<ISourceAppContext>();
            return appContext.App();
        }

    }
}
