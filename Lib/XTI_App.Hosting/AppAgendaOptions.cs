namespace XTI_App.Hosting;

public sealed class AppAgendaOptions
{
    public static readonly string AppAgenda = nameof(AppAgenda);
    public ImmediateAppAgendaItemOptions[] ImmediateItems { get; set; } = new ImmediateAppAgendaItemOptions[0];
    public ScheduledAppAgendaItemOptions[] ScheduledItems { get; set; } = new ScheduledAppAgendaItemOptions[0];
    public AlwaysRunningAppAgendaItemOptions[] AlwaysRunningItems { get; set; } = new AlwaysRunningAppAgendaItemOptions[0];
}