using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Fakes;

namespace XTI_App.Tests
{
    public static class Extensions
    {
        public static void AddServicesForTests(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFakesForXtiApp(configuration);
            services.AddSingleton(_ => FakeInfo.AppKey);
            services.AddSingleton(_ => AppVersionKey.Current);
            services.AddScoped<AppApiFactory, FakeAppApiFactory>();
            services.AddScoped<FakeAppSetup>();
            services.AddScoped<IAppSetup>(sp => sp.GetService<FakeAppSetup>());
        }

        public static Task Setup(this IServiceProvider services)
        {
            var setup = services.GetService<FakeAppSetup>();
            return setup.Run(AppVersionKey.Current);
        }

        public static FakeApp FakeApp(this IServiceProvider services)
        {
            var setup = services.GetService<FakeAppSetup>();
            return setup.App;
        }
    }
}
