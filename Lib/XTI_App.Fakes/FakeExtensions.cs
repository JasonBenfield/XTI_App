using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_Core;
using XTI_Core.Fakes;
using XTI_TempLog.Fakes;

namespace XTI_App.Fakes
{
    public static class FakeExtensions
    {
        public static void AddFakesForXtiApp(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddDataProtection();
            services.AddSingleton<FakeClock>();
            services.AddSingleton<Clock>(sp => sp.GetService<FakeClock>());
            services.Configure<AppOptions>(configuration.GetSection(AppOptions.App));
            services.AddScoped<IAppApiUser, AppApiUser>();
            services.AddScoped(sp =>
            {
                var appKey = sp.GetService<AppKey>();
                return new FakeXtiPathAccessor
                (
                    new XtiPath
                    (
                        appKey.Name,
                        AppVersionKey.Current,
                        new ResourceGroupName("Home"),
                        new ResourceName("Index"),
                        ModifierKey.Default
                    )
                );
            });
            services.AddScoped<IXtiPathAccessor>(sp => sp.GetService<FakeXtiPathAccessor>());
            services.AddScoped(sp => sp.GetService<IXtiPathAccessor>().Value());
            services.AddScoped(sp => sp.GetService<XtiPath>().Version);
            services.AddScoped<IHashedPasswordFactory, FakeHashedPasswordFactory>();
            services.AddSingleton<FakeAppContext>();
            services.AddSingleton<ISourceAppContext>(sp => sp.GetService<FakeAppContext>());
            services.AddSingleton<CachedAppContext>();
            services.AddSingleton<IAppContext>(sp => sp.GetService<CachedAppContext>());
            services.AddSingleton<FakeUserContext>();
            services.AddSingleton<ISourceUserContext>(sp => sp.GetService<FakeUserContext>());
            services.AddSingleton<CachedUserContext>();
            services.AddSingleton<IUserContext>(sp => sp.GetService<CachedUserContext>());
            services.AddScoped(sp =>
            {
                var factory = sp.GetService<AppApiFactory>();
                var user = sp.GetService<IAppApiUser>();
                return factory.Create(user);
            });
            services.AddFakeTempLogServices();
        }
    }
}
