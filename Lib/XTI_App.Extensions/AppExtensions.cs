using MainDB.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.EfApi;
using XTI_App.Secrets;
using XTI_Core;
using XTI_Secrets.Extensions;
using XTI_TempLog.Extensions;

namespace XTI_App.Extensions
{
    public static class AppExtensions
    {
        public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.Configure<AppOptions>(configuration.GetSection(AppOptions.App));
            services.AddXtiDataProtection();
            services.AddMainDbContextForSqlServer(configuration);
            services.AddSingleton<Clock, UtcClock>();
            services.AddScoped<AppFactory>();
            services.AddFileSecretCredentials();
            services.AddScoped<SystemUserCredentials>();
            services.AddScoped<ISystemUserCredentials>(sp =>
            {
                var cache = sp.GetService<IMemoryCache>();
                var source = sp.GetService<SystemUserCredentials>();
                return new CachedSystemUserCredentials(cache, source);
            });
            services.AddScoped<SystemUserContext>();
            services.AddScoped<CachedSystemUserContext>();
            services.AddScoped<ISourceAppContext, DefaultAppContext>();
            services.AddScoped<IAppContext>(sp =>
            {
                var memoryCache = sp.GetService<IMemoryCache>();
                return new CachedAppContext(sp, memoryCache);
            });
            services.AddScoped<CachedUserContext>();
            services.AddScoped<IUserContext>(sp => sp.GetService<CachedUserContext>());
            services.AddScoped<ICachedUserContext>(sp => sp.GetService<CachedUserContext>());
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
