using MainDB.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_Secrets.Extensions;

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
            services.AddScoped<SystemUserContext>();
        }
    }
}
