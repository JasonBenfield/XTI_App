using XTI_App.Abstractions;
using XTI_Schedule;

namespace XTI_App.Hosting;

public sealed class ScheduledAppAgendaItem : AppAgendaItem
{
    public ScheduledAppAgendaItem
    (
        ResourceGroupName groupName,
        ResourceName actionName,
        bool isDisabled,
        AggregateSchedule schedule,
        ScheduledActionTypes type,
        TimeSpan delayAfterStart,
        TimeSpan interval
    )
    : base(groupName, actionName, isDisabled)
    {
        Schedule = schedule;
        Type = type;
        DelayAfterStart = delayAfterStart;
        Interval = interval;
    }

    public AggregateSchedule Schedule { get; }
    public ScheduledActionTypes Type { get; }
    public TimeSpan DelayAfterStart { get; }
    public TimeSpan Interval { get; }
}