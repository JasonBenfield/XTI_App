using Microsoft.Extensions.Hosting;

namespace XTI_App.Hosting;

public sealed class AppAgendaHostedService : IHostedService
{
    private readonly AppAgenda agenda;

    public AppAgendaHostedService(AppAgenda agenda)
    {
        this.agenda = agenda;
    }

    public Task StartAsync(CancellationToken cancellationToken) => agenda.Start(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => agenda.Stop();
}