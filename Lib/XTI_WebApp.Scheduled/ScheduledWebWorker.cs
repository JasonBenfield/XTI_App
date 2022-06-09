using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Hosting;

namespace XTI_WebApp.Scheduled;

public sealed class ScheduledWebWorker : BackgroundService
{
    private readonly IServiceProvider sp;
    private AppAgenda? appAgenda;

    public ScheduledWebWorker(IServiceProvider sp)
    {
        this.sp = sp;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = sp.CreateScope();
        var pathAccessor = scope.ServiceProvider.GetRequiredService<ActionRunnerXtiPathAccessor>();
        if (pathAccessor.Value().IsCurrentVersion())
        {
            appAgenda = scope.ServiceProvider.GetRequiredService<AppAgenda>();
            await appAgenda.Start(stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (appAgenda != null)
        {
            await appAgenda.Stop(cancellationToken);
        }
        await base.StopAsync(cancellationToken);
    }
}