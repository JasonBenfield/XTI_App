using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using XTI_App;
using XTI_App.DB;
using XTI_App.EF;
using XTI_App.Extensions;
using XTI_Core;
using XTI_Secrets;

namespace XTI_Tool.Extensions
{
    public static class ToolExtensions
    {
        public static bool IsTest(this IHostEnvironment env) => env.IsEnvironment("Test");
        public static bool IsDevOrTest(this IHostEnvironment env) => env.IsDevelopment() || env.IsTest();

        public static void AddConsoleAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppOptions>(configuration.GetSection(AppOptions.App));
            services.Configure<DbOptions>(configuration.GetSection(DbOptions.DB));
            services.Configure<SecretOptions>(configuration.GetSection(SecretOptions.Secret));
            var secretOptions = configuration.GetSection(SecretOptions.Secret).Get<SecretOptions>();
            services
                .AddDataProtection
                (
                    options => options.ApplicationDiscriminator = secretOptions.ApplicationName
                )
                .PersistKeysToFileSystem(new DirectoryInfo(secretOptions.KeyDirectoryPath))
                .SetApplicationName(secretOptions.ApplicationName);
            services.AddAppDbContextForSqlServer(configuration);
            services.AddScoped<AppFactory, EfAppFactory>();
            services.AddScoped<Clock, UtcClock>();
        }
    }
}
