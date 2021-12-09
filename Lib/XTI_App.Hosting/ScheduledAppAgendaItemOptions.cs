using XTI_Schedule;

namespace XTI_App.Hosting;

public sealed class ScheduledAppAgendaItemOptions
{
    public string GroupName { get; set; } = "";
    public string ActionName { get; set; } = "";
    public ScheduledActionTypes Type { get; set; } = ScheduledActionTypes.Continuous;
    public TimeSpan DelayAfterStart { get; set; }
    public TimeSpan Interval { get; set; }
    public ScheduleOptions Schedule { get; set; } = new ScheduleOptions();
    public bool IsDisabled { get; set; }
}