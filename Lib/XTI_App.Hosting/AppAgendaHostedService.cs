using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace XTI_App.Hosting;

public sealed class AppAgendaHostedService : IHostedService
{
    private readonly AppAgenda agenda;

    public AppAgendaHostedService(IServiceProvider sp)
    {
        var scope = sp.CreateScope();
        agenda = scope.ServiceProvider.GetRequiredService<AppAgenda>();
    }

    public Task StartAsync(CancellationToken cancellationToken) => agenda.Start(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => agenda.Stop(cancellationToken);
}