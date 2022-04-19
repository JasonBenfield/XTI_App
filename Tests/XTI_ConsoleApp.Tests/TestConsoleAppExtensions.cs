using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Api;
using XTI_App.Hosting;
using XTI_ConsoleApp.Fakes;
using XTI_Core;

namespace XTI_ConsoleApp.Tests;

public static class TestConsoleAppExtensions
{
    public static void AddTestConsoleAppServices(this IServiceCollection services, Action<IServiceProvider, AppAgendaBuilder> build)
    {
        services.AddSingleton(sp => XtiEnvironment.Parse(sp.GetRequiredService<IHostEnvironment>().EnvironmentName));
        services.AddFakeConsoleAppServices(build);
        services.AddSingleton<Counter>();
        services.AddSingleton<TestOptions>();
        services.AddScoped<IAppApiUser, AppApiSuperUser>();
        services.AddScoped(_ => TestAppInfo.AppKey);
        services.AddScoped<AppApiFactory, TestApiFactory>();
        services.AddScoped(sp => sp.GetRequiredService<AppApiFactory>().CreateForSuperUser());
        services.AddScoped(sp => (TestApi)sp.GetRequiredService<IAppApi>());
    }
}