using MainDB.EF;
using MainDB.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.TestFakes;
using XTI_Core;
using XTI_Core.Extensions;

namespace XTI_App.IntegrationTests
{
    public static class Extensions
    {
        public static void AddXtiTestServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddMainDbContextForSqlServer(config);
            services.AddSingleton<Clock, UtcClock>();
            services.AddSingleton(_ => FakeInfo.AppKey);
            services.AddScoped<AppFactory>();
            services.AddScoped<MainDbReset>();
            services.AddScoped<FakeAppSetup>();
            services.AddScoped<FakeAppApiFactory>();
            services.AddScoped(sp =>
            {
                var factory = sp.GetService<FakeAppApiFactory>();
                return factory.CreateForSuperUser();
            });
        }

        public static async Task Reset(this IServiceProvider services)
        {
            var hostEnv = services.GetService<IHostEnvironment>();
            if (hostEnv.IsTest())
            {
                var mainDbReset = services.GetService<MainDbReset>();
                await mainDbReset.Run();
            }
            var setup = services.GetService<FakeAppSetup>();
            await setup.Run(AppVersionKey.Current);
        }

        public static Task<App> FakeApp(this IServiceProvider services)
        {
            var factory = services.GetService<AppFactory>();
            return factory.Apps().App(FakeInfo.AppKey);
        }
    }
}
