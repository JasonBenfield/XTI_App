using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App.Hosting;

public sealed class ImmediateActionWorker : BackgroundService, IWorker
{
    private readonly IServiceProvider sp;
    private readonly XtiEnvironment environment;
    private readonly XtiBasePath xtiBasePath;
    private readonly ImmediateAppAgendaItem[] agendaItems;

    public ImmediateActionWorker
    (
        IServiceProvider sp,
        XtiEnvironment environment,
        XtiBasePath xtiBasePath,
        ImmediateAppAgendaItem[] agendaItems
    )
    {
        this.sp = sp;
        this.environment = environment;
        this.xtiBasePath = xtiBasePath;
        this.agendaItems = agendaItems.Where(item => item.IsEnabled).ToArray();
    }

    public bool HasStopped { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var agendaItem in agendaItems)
        {
            var actionExecutor = new ActionRunner
            (
                sp,
                environment, 
                xtiBasePath,
                agendaItem.GroupName.DisplayText,
                agendaItem.ActionName.DisplayText
            );
            await actionExecutor.Run(stoppingToken);
        }
        HasStopped = true;
    }
}