using MainDB.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_DB;
using XTI_Secrets.Extensions;

namespace XTI_Tool.Extensions
{
    public static class ToolExtensions
    {
        public static void AddConsoleAppServices(this IServiceCollection services, IHostEnvironment hostEnv, IConfiguration configuration)
        {
            services.Configure<AppOptions>(configuration.GetSection(AppOptions.App));
            services.Configure<DbOptions>(configuration.GetSection(DbOptions.DB));
            services.AddXtiDataProtection();
            services.AddMainDbContextForSqlServer(configuration);
            services.AddScoped<AppFactory>();
            services.AddScoped<Clock, UtcClock>();
        }
    }
}
