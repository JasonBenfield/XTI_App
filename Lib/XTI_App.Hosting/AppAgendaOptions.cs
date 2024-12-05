using XTI_App.Api;

namespace XTI_App.Hosting;

public sealed class AppAgendaOptions
{
    public static readonly string AppAgenda = nameof(AppAgenda);
    public ImmediateAppAgendaItemOptions[] ImmediateItems { get; set; } = [];
    public ScheduledAppAgendaItemOptions[] ScheduledItems { get; set; } = [];
    public AlwaysRunningAppAgendaItemOptions[] AlwaysRunningItems { get; set; } = [];
}