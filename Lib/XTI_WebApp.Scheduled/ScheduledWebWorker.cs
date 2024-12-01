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

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = sp.CreateScope();
        appAgenda = scope.ServiceProvider.GetRequiredService<AppAgenda>();
        return appAgenda.Start(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (appAgenda != null)
        {
            await appAgenda.Stop(cancellationToken);
            appAgenda = null;
        }
        await base.StopAsync(cancellationToken);
    }
}