using Microsoft.Extensions.DependencyInjection;
using XTI_App.Hosting;

namespace XTI_WebApp.Scheduled;

public static class ScheduledExtensions
{
    public static void AddScheduledWebServices(this IServiceCollection services, Action<IServiceProvider, AppAgendaBuilder> build)
    {
        services.AddScoped<IActionRunnerFactory, WebActionRunnerFactory>();
        services.AddSingleton<ScheduledAppEnvironmentContext>();
        services.AddAppAgenda(build);
        services.AddHostedService<AppAgendaHostedService>();
    }
}