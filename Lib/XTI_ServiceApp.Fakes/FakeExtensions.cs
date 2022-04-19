using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_App.Hosting;
using XTI_Core;
using XTI_Core.Fakes;
using XTI_TempLog;
using XTI_TempLog.Fakes;

namespace XTI_ConsoleApp.Fakes;

public static class FakeExtensions
{
    public static void AddFakeServiceAppServices(this IServiceCollection services, Action<IServiceProvider, AppAgendaBuilder> build)
    {
        services.AddDataProtection();
        services.AddAppAgenda(build);
        services.AddSingleton(_ => AppVersionKey.Current);
        services.AddSingleton<IClock, FakeClock>();
        services.AddScoped<ActionRunnerXtiPathAccessor>();
        services.AddScoped<IXtiPathAccessor>(sp => sp.GetRequiredService<ActionRunnerXtiPathAccessor>());
        services.AddSingleton(sp => sp.GetRequiredService<IXtiPathAccessor>().Value());
        services.AddScoped<IActionRunnerFactory, ActionRunnerFactory>();
        services.AddSingleton<IAppEnvironmentContext, FakeAppEnvironmentContext>();
        services.AddScoped<FakeUserContext>();
        services.AddScoped<ISourceUserContext>(sp => sp.GetRequiredService<FakeUserContext>());
        services.AddScoped<IUserContext>(sp => sp.GetRequiredService<ISourceUserContext>());
        services.AddScoped<FakeAppContext>();
        services.AddScoped<ISourceAppContext>(sp => sp.GetRequiredService<FakeAppContext>());
        services.AddScoped<IAppContext>(sp => sp.GetRequiredService<ISourceAppContext>());
        services.AddScoped<IAppApiUser, AppApiUser>();
        services.AddFakeTempLogServices();
        services.AddScoped(sp =>
        {
            var factory = sp.GetRequiredService<AppApiFactory>();
            var user = sp.GetRequiredService<IAppApiUser>();
            return factory.Create(user);
        });
        services.AddHostedService<AppAgendaHostedService>();
    }
}