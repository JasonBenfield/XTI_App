using MainDB.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MainDB.Extensions;
using XTI_Core;
using XTI_App.TestFakes;
using Microsoft.Extensions.Hosting;
using XTI_Core.Extensions;

namespace XTI_App.IntegrationTests
{
    public static class Extensions
    {
        public static void AddXtiTestServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAppDbContextForSqlServer(config);
            services.AddSingleton<Clock, UtcClock>();
            services.AddSingleton(_ => FakeAppKey.AppKey);
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
            await setup.Run();
        }

        public static Task<App> FakeApp(this IServiceProvider services)
        {
            var factory = services.GetService<AppFactory>();
            return factory.Apps().App(FakeAppKey.AppKey);
        }
    }
}
