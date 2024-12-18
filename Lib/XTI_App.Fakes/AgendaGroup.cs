using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class AgendaGroup : AppApiGroupWrapper
{
    public AgendaGroup(AppApiGroup source, IServiceProvider sp) : base(source)
    {
        FirstAgendaItem = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(FirstAgendaItem))
            .WithExecution(() => new FirstAgendaItemAction(sp.GetRequiredService<FirstAgendaItemCounter>()))
            .Build();
        SecondAgendaItem = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(SecondAgendaItem))
            .WithExecution(() => new SecondAgendaItemAction())
            .Build();
        ThirdAgendaItem = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(ThirdAgendaItem))
            .WithExecution(() => new ThirdAgendaItemAction())
            .Build();
        Stop = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(Stop))
            .WithExecution(() => new StopApplicationAction(sp.GetRequiredService<IHostApplicationLifetime>()))
            .Build();
    }

    public AppApiAction<EmptyRequest, EmptyActionResult> FirstAgendaItem { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> SecondAgendaItem { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> ThirdAgendaItem { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Stop { get; }
}