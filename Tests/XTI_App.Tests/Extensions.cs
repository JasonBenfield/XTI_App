using MainDB.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.EfApi;
using XTI_App.Extensions;
using XTI_App.Fakes;
using XTI_App.TestFakes;
using XTI_Core;
using XTI_Core.Fakes;

namespace XTI_App.Tests
{
    public static class Extensions
    {
        public static void AddServicesForTests(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddMainDbContextForInMemory();
            services.AddScoped<AppFactory>();
            services.AddSingleton<FakeClock>();
            services.AddSingleton<Clock>(sp => sp.GetService<FakeClock>());
            services.AddScoped<ISourceAppContext, DefaultAppContext>();
            services.AddScoped<IAppContext, CachedAppContext>();
            services.AddScoped<ISourceUserContext, FakeUserContext>();
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
        }

        public static Task Setup(this IServiceProvider services)
        {
            var setup = services.GetService<FakeAppSetup>();
            return setup.Run(AppVersionKey.Current);
        }

        public static Task<App> FakeApp(this IServiceProvider services)
        {
            var factory = services.GetService<AppFactory>();
            return factory.Apps().App(FakeInfo.AppKey);
        }
    }
}
