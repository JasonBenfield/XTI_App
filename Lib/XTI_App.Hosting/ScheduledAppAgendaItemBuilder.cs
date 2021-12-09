using XTI_App.Abstractions;
using XTI_Core;
using XTI_Schedule;

namespace XTI_App.Hosting;

public sealed class ScheduledAppAgendaItemBuilder : IAppAgendaItemBuilder
{
    private ResourceGroupName groupName = ResourceGroupName.Unknown;
    private ResourceName actionName = ResourceName.Unknown;
    private bool isEnabled = true;
    private readonly List<Schedule> schedules = new List<Schedule>();
    private ScheduledActionTypes type = ScheduledActionTypes.Continuous;
    private TimeSpan delayAfterStart = new TimeSpan();
    private TimeSpan interval = TimeSpan.FromDays(1);

    internal ScheduledAppAgendaItemBuilder() { }

    internal ScheduledAppAgendaItemBuilder(AlwaysRunningAppAgendaItemOptions options)
        : this
        (
             new ScheduledAppAgendaItemOptions
             {
                 GroupName = options.GroupName,
                 ActionName = options.ActionName,
                 Type = ScheduledActionTypes.Continuous,
                 DelayAfterStart = options.DelayAfterStart,
                 Interval = options.Interval,
                 IsDisabled = options.IsDisabled,
                 Schedule = new ScheduleOptions
                 {
                     WeeklySchedules = new[]
                    {
                            new WeeklyScheduleOptions
                            {
                                Days = new[]
                                {
                                    DayOfWeek.Sunday,
                                    DayOfWeek.Monday,
                                    DayOfWeek.Tuesday,
                                    DayOfWeek.Wednesday,
                                    DayOfWeek.Thursday,
                                    DayOfWeek.Friday,
                                    DayOfWeek.Saturday
                                },
                                TimeRanges = new []
                                {
                                    new TimeRangeOptions
                                    {
                                        Start = new Time(),
                                        Duration = TimeSpan.FromHours(24)
                                    }
                                }
                            }
                    }
                 }
             }
        )
    {
    }

    internal ScheduledAppAgendaItemBuilder(ScheduledAppAgendaItemOptions options)
    {
        Action(new ResourceGroupName(options.GroupName), new ResourceName(options.ActionName));
        if (options.IsDisabled)
        {
            Disable();
        }
        if (options.Type == ScheduledActionTypes.PeriodicUntilSuccess)
        {
            RunUntilSuccess();
        }
        DelayAfterStart(options.DelayAfterStart);
        Interval(options.Interval);
        schedules.AddRange(options.Schedule.ToSchedules());
    }

    public bool HasAction(string groupName, string actionName)
        => this.groupName.Equals(groupName) && this.actionName.Equals(actionName);

    public ScheduledAppAgendaItemBuilder Action(XtiPath path) => Action(path.Group, path.Action);

    public ScheduledAppAgendaItemBuilder Action(ResourceGroupName groupName, ResourceName actionName)
    {
        this.groupName = groupName;
        this.actionName = actionName;
        return this;
    }

    public ScheduledAppAgendaItemBuilder Disable()
    {
        isEnabled = false;
        return this;
    }

    public ScheduledAppAgendaItemBuilder RunContinuously()
    {
        type = ScheduledActionTypes.Continuous;
        return this;
    }

    public ScheduledAppAgendaItemBuilder RunUntilSuccess()
    {
        type = ScheduledActionTypes.PeriodicUntilSuccess;
        return this;
    }

    public ScheduledAppAgendaItemBuilder AddSchedule(Schedule schedule)
    {
        schedules.Add(schedule);
        return this;
    }

    public ScheduledAppAgendaItemBuilder DelayAfterStart(TimeSpan delayAfterStart)
    {
        this.delayAfterStart = delayAfterStart;
        return this;
    }

    public ScheduledAppAgendaItemBuilder Interval(TimeSpan interval)
    {
        this.interval = interval;
        return this;
    }

    public ScheduledAppAgendaItem Build()
        => new ScheduledAppAgendaItem
        (
            groupName,
            actionName,
            isEnabled,
            new AggregateSchedule(schedules.ToArray()),
            type,
            delayAfterStart,
            interval
        );

    AppAgendaItem IAppAgendaItemBuilder.Build() => Build();
}