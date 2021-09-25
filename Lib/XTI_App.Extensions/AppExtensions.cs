using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Secrets;
using XTI_Core;
using XTI_Secrets.Extensions;
using XTI_TempLog.Extensions;

namespace XTI_App.Extensions
{
    public static class AppExtensions
    {
        public static void AddAppServices(this IServiceCollection services, IHostEnvironment hostEnv, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.Configure<AppOptions>(configuration.GetSection(AppOptions.App));
            services.AddSingleton<Clock, UtcClock>();
            services.AddSingleton<XtiFolder>();
            services.AddFileSecretCredentials(hostEnv);
            services.AddScoped(sp => sp.GetService<IXtiPathAccessor>().Value());
            services.AddScoped(sp => sp.GetService<XtiPath>().Version);
            services.AddScoped<IAppContext, CachedAppContext>();
            services.AddScoped<CachedUserContext>();
            services.AddScoped<IUserContext>(sp => sp.GetService<CachedUserContext>());
            services.AddScoped<ICachedUserContext>(sp => sp.GetService<CachedUserContext>());
            services.AddScoped<SystemUserCredentials>();
            services.AddScoped<ISystemUserCredentials>(sp =>
            {
                var cache = sp.GetService<IMemoryCache>();
                var sourceCredentials = sp.GetService<SystemUserCredentials>();
                return new CachedSystemUserCredentials(cache, sourceCredentials);
            });
            services.AddScoped<SystemUserContext>();
            services.AddScoped<ISystemUserContext, CachedSystemUserContext>();
            services.AddScoped<IAppApiUser, AppApiUser>();
            services.AddScoped(sp =>
            {
                var factory = sp.GetService<AppApiFactory>();
                var user = sp.GetService<IAppApiUser>();
                return factory.Create(user);
            });
            services.AddTempLogServices(configuration);
        }
    }
}
