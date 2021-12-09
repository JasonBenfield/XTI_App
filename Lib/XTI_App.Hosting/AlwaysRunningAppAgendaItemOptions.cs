namespace XTI_App.Hosting;

public sealed class AlwaysRunningAppAgendaItemOptions
{
    public string GroupName { get; set; } = "";
    public string ActionName { get; set; } = "";
    public TimeSpan DelayAfterStart { get; set; }
    public TimeSpan Interval { get; set; }
    public bool IsDisabled { get; set; }
}