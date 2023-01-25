using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class AgendaGroup : AppApiGroupWrapper
{
    public AgendaGroup(AppApiGroup source, IServiceProvider sp) : base(source)
    {
        FirstAgendaItem = source.AddAction
        (
            nameof(FirstAgendaItem),
            () => new FirstAgendaItemAction(sp.GetRequiredService<FirstAgendaItemCounter>())
        );
        SecondAgendaItem = source.AddAction
        (
            nameof(SecondAgendaItem),
            () => new SecondAgendaItemAction()
        );
        ThirdAgendaItem = source.AddAction
        (
            nameof(ThirdAgendaItem),
            () => new ThirdAgendaItemAction()
        );
        Stop = source.AddAction
        (
            nameof(Stop),
            () => new StopApplicationAction(sp.GetRequiredService<IHostApplicationLifetime>())
        );
    }

    public AppApiAction<EmptyRequest, EmptyActionResult> FirstAgendaItem { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> SecondAgendaItem { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> ThirdAgendaItem { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Stop { get; }
}