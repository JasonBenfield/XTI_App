using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_App.Fakes;
using XTI_App.Hosting;
using XTI_TempLog;
using XTI_TempLog.Fakes;

namespace XTI_App.Tests
{
    public static class Extensions
    {
        public static void AddServicesForTests(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFakesForXtiApp(configuration);
            services.AddSingleton(_ => FakeInfo.AppKey);
            services.AddSingleton(_ => AppVersionKey.Current);
            services.AddScoped<FakeAppApiFactory>();
            services.AddScoped<AppApiFactory>(sp => sp.GetService<FakeAppApiFactory>());
            services.AddScoped(sp => sp.GetService<FakeAppApiFactory>().CreateForSuperUser());
            services.AddScoped<FakeAppSetup>();
            services.AddScoped<IAppSetup>(sp => sp.GetService<FakeAppSetup>());
            services.AddScoped<IActionRunnerFactory, ActionRunnerFactory>();
            services.AddScoped<IAppEnvironmentContext, FakeAppEnvironmentContext>();
            services.AddScoped<ActionRunnerXtiPathAccessor>();
            services.AddThrottledLog<FakeAppApi>
            (
                (api, log) => log
                    .Throttle(api.Agenda.SecondAgendaItem)
                    .Requests().For(5).Minutes()
                    .AndThrottle(api.Agenda.ThirdAgendaItem)
                    .Requests().For(30).Minutes()
            );
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
