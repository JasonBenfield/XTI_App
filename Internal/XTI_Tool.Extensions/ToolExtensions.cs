using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using XTI_App;
using XTI_App.EF;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using XTI_Secrets;
using XTI_Core;
using XTI_App.DB;

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
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var appDbOptions = sp.GetService<IOptions<DbOptions>>().Value;
                var hostEnvironment = sp.GetService<IHostEnvironment>();
                options.UseSqlServer(new AppConnectionString(appDbOptions, hostEnvironment.EnvironmentName).Value());
                if (hostEnvironment.IsDevOrTest())
                {
                    options.EnableSensitiveDataLogging();
                }
                else
                {
                    options.EnableSensitiveDataLogging(false);
                }
            });
            services.AddScoped<AppFactory, EfAppFactory>();
            services.AddScoped<Clock, UtcClock>();
        }
    }
}
