using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_App.Fakes;
using XTI_Core;
using XTI_Core.Fakes;

namespace XTI_App.Tests
{
    public static class Extensions
    {
        public static void AddServicesForTests(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<FakeClock>();
            services.AddSingleton<Clock>(sp => sp.GetService<FakeClock>());
            services.AddScoped<FakeAppContext>();
            services.AddScoped<ISourceAppContext>(sp => sp.GetService<FakeAppContext>());
            services.AddScoped<IAppContext, CachedAppContext>();
            services.AddScoped<FakeUserContext>();
            services.AddScoped<ISourceUserContext>(sp => sp.GetService<FakeUserContext>());
            services.AddScoped<IUserContext, CachedUserContext>();
            services.AddSingleton(_ => FakeInfo.AppKey);
            services.AddSingleton(_ => AppVersionKey.Current);
            services.AddScoped<IXtiPathAccessor>(sp =>
            {
                var appKey = sp.GetService<AppKey>();
                return new FakeXtiPathAccessor(new XtiPath(appKey.Name.DisplayText));
            });
            services.AddScoped<IAppApiUser, AppApiUser>();
            services.AddScoped<AppApiFactory, FakeAppApiFactory>();
            services.AddScoped(sp =>
            {
                var factory = sp.GetService<AppApiFactory>();
                var apiUser = sp.GetService<IAppApiUser>();
                return factory.Create(apiUser);
            });
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
