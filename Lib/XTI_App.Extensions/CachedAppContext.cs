using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using Microsoft.Extensions.DependencyInjection;

namespace XTI_App.Extensions
{
    public sealed class CachedAppContext : IAppContext
    {
        private readonly IServiceProvider services;
        private readonly IMemoryCache cache;

        public CachedAppContext(IServiceProvider services, IMemoryCache cache)
        {
            this.services = services;
            this.cache = cache;
        }

        public async Task<IApp> App()
        {
            if (!cache.TryGetValue("xti_app", out CachedApp cachedApp))
            {
                var app = await getSourceAppContext().App();
                cachedApp = new CachedApp(services, app);
                cache.Set
                (
                    "xti_app",
                    cachedApp,
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = new TimeSpan(4, 0, 0)
                    }
                );
            }
            return cachedApp;
        }

        public async Task<IAppVersion> Version()
        {
            if (!cache.TryGetValue("xti_version", out CachedAppVersion cachedVersion))
            {
                var version = await getSourceAppContext().Version();
                cachedVersion = new CachedAppVersion(services, version);
                cache.Set
                (
                    "xti_version",
                    cachedVersion,
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = new TimeSpan(4, 0, 0)
                    }
                );
            }
            return cachedVersion;
        }

        private ISourceAppContext getSourceAppContext() => services.GetService<ISourceAppContext>();
    }
}
