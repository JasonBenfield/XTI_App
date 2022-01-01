using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_Core;
using XTI_Core.Fakes;
using XTI_TempLog.Fakes;

namespace XTI_App.Fakes;

public static class FakeExtensions
{
    public static void AddFakesForXtiApp(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddDataProtection();
        services.AddSingleton<FakeClock>();
        services.AddSingleton<IClock>(sp => sp.GetRequiredService<FakeClock>());
        services.Configure<AppOptions>(configuration.GetSection(AppOptions.App));
        services.AddScoped<IAppApiUser, AppApiUser>();
        services.AddScoped(sp =>
        {
            var appKey = sp.GetRequiredService<AppKey>();
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
        services.AddScoped<IXtiPathAccessor>(sp => sp.GetRequiredService<FakeXtiPathAccessor>());
        services.AddScoped(sp => sp.GetRequiredService<IXtiPathAccessor>().Value());
        services.AddScoped(sp => sp.GetRequiredService<XtiPath>().Version);
        services.AddScoped<IHashedPasswordFactory, FakeHashedPasswordFactory>();
        services.AddSingleton<FakeAppContext>();
        services.AddSingleton<ISourceAppContext>(sp => sp.GetRequiredService<FakeAppContext>());
        services.AddScoped<CachedAppContext>();
        services.AddScoped<IAppContext>(sp => sp.GetRequiredService<FakeAppContext>());
        services.AddSingleton<FakeUserContext>();
        services.AddSingleton<ISourceUserContext>(sp => sp.GetRequiredService<FakeUserContext>());
        services.AddScoped<CachedUserContext>();
        services.AddScoped<IUserContext>(sp => sp.GetRequiredService<FakeUserContext>());
        services.AddScoped(sp =>
        {
            var factory = sp.GetRequiredService<AppApiFactory>();
            var user = sp.GetRequiredService<IAppApiUser>();
            return factory.Create(user);
        });
        services.AddSingleton<FirstAgendaItemCounter>();
        services.AddFakeTempLogServices();
    }
}