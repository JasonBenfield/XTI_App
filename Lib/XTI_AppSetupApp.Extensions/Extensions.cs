using MainDB.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XTI_App;
using XTI_Core;

namespace XTI_AppSetupApp.Extensions
{
    public static class Extensions
    {
        public static void AddAppSetupServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAppDbContextForSqlServer(config);
            services.AddScoped<AppFactory>();
            services.AddScoped<Clock, UtcClock>();
            services.Configure<SetupOptions>(config.GetSection(SetupOptions.Setup));
            services.AddHostedService<SetupHostedService>();
        }
    }
}
