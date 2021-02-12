using MainDB.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.TestFakes;
using XTI_Core;
using XTI_Core.Fakes;

namespace XTI_App.Tests
{
    public static class Extensions
    {
        public static void AddServicesForTests(this IServiceCollection services)
        {
            services.AddAppDbContextForInMemory();
            services.AddSingleton<AppFactory>();
            services.AddSingleton<FakeClock>();
            services.AddSingleton<Clock>(sp => sp.GetService<FakeClock>());
        }

        public static Task Setup(this IServiceProvider services)
        {
            var factory = services.GetService<AppFactory>();
            var clock = services.GetService<FakeClock>();
            var setup = new FakeAppSetup(factory, clock);
            return setup.Run(AppVersionKey.Current);
        }

        public static Task<App> FakeApp(this IServiceProvider services)
        {
            var factory = services.GetService<AppFactory>();
            return factory.Apps().App(FakeInfo.AppKey);
        }
    }
}
