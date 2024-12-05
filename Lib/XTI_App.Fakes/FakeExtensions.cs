using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_Core;
using XTI_Core.Fakes;
using XTI_Secrets.Extensions;
using XTI_TempLog.Fakes;

namespace XTI_App.Fakes;

public static class FakeExtensions
{
    public static void AddFakesForXtiApp(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddXtiDataProtection();
        services.AddSingleton<FakeInstallationIDAccessor>();
        services.AddSingleton<InstallationIDAccessor>(sp => sp.GetRequiredService<FakeInstallationIDAccessor>());
        services.AddSingleton<FakeModifierKeyAccessor>();
        services.AddSingleton<IModifierKeyAccessor>(sp => sp.GetRequiredService<FakeModifierKeyAccessor>());
        services.AddSingleton<FakeHostEnvironment>();
        services.AddSingleton<IHostEnvironment>(sp => sp.GetRequiredService<FakeHostEnvironment>());
        services.AddSingleton<FakeClock>();
        services.AddSingleton<IClock>(sp => sp.GetRequiredService<FakeClock>());
        services.AddScoped<IAppApiUser, AppApiUser>();
        services.AddSingleton(sp => AppVersionKey.Current);
        services.AddSingleton
        (
            sp => new XtiBasePath
            (
                sp.GetRequiredService<AppKey>(), 
                sp.GetRequiredService<AppVersionKey>()
            )
        );
        services.AddScoped<IHashedPasswordFactory, FakeHashedPasswordFactory>();
        services.AddSingleton<FakeAppContextFactory>();
        services.AddSingleton
        (
            sp => sp.GetRequiredService<FakeAppContextFactory>().Create(sp.GetRequiredService<AppKey>())
        );
        services.AddSingleton<ISourceAppContext>(sp => sp.GetRequiredService<FakeAppContext>());
        services.AddScoped<CachedAppContext>();
        services.AddScoped<IAppContext>(sp => sp.GetRequiredService<FakeAppContext>());
        services.AddSingleton<FakeCurrentUserName>();
        services.AddSingleton<ICurrentUserName>(sp => sp.GetRequiredService<FakeCurrentUserName>());
        services.AddSingleton<FakeUserContext>();
        services.AddSingleton<ISourceUserContext>(sp => sp.GetRequiredService<FakeUserContext>());
        services.AddScoped<CachedUserContext>();
        services.AddScoped<IUserContext>(sp => sp.GetRequiredService<FakeUserContext>());
        services.AddScoped<ICachedUserContext>(sp => sp.GetRequiredService<CachedUserContext>());
        services.AddScoped<CurrentUserAccess>();
        services.AddSingleton<FakeError>();
        services.AddScoped<LogRequestDataAction>();
        services.AddScoped<LogRequestDataOnErrorAction>();
        services.AddScoped<LogResultDataAction>();
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