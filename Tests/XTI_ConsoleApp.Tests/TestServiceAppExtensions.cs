using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Api;
using XTI_App.Hosting;
using XTI_ConsoleApp.Fakes;
using XTI_Core;

namespace XTI_ConsoleApp.Tests;

public static class TestServiceAppExtensions
{
    public static void AddTestServiceAppServices(this IServiceCollection services, Action<IServiceProvider, AppAgendaBuilder> build)
    {
        services.AddMemoryCache();
        services.AddSingleton(sp => XtiEnvironment.Parse(sp.GetRequiredService<IHostEnvironment>().EnvironmentName));
        services.AddFakeServiceAppServices(build);
        services.AddSingleton<Counter>();
        services.AddSingleton<TestOptions>();
        services.AddScoped<IAppApiUser, AppApiSuperUser>();
        services.AddScoped(_ => TestAppInfo.AppKey);
        services.AddScoped<AppApiFactory, TestApiFactory>();
        services.AddScoped(sp => (TestApi)sp.GetRequiredService<IAppApi>());
    }
}