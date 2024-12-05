using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Schedule;

namespace XTI_App.Hosting;

public sealed class ScheduledAppAgendaItemBuilder : IAppAgendaItemBuilder
{
    private bool isEnabled = true;
    private readonly List<Schedule> schedules = new();
    private ScheduledActionTypes type = ScheduledActionTypes.Continuous;
    private TimeSpan delayAfterStart = new();
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
                     WeeklySchedules =
                    [
                        new WeeklyScheduleOptions
                        {
                            Days =
                            [
                                DayOfWeek.Sunday,
                                DayOfWeek.Monday,
                                DayOfWeek.Tuesday,
                                DayOfWeek.Wednesday,
                                DayOfWeek.Thursday,
                                DayOfWeek.Friday,
                                DayOfWeek.Saturday
                            ],
                            TimeRanges =
                            [
                                new TimeRangeOptions
                                {
                                    Start = new TimeOnly(),
                                    Duration = TimeSpan.FromHours(24)
                                }
                            ]
                        }
                    ]
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

    public ResourceGroupName GroupName { get; private set; } = ResourceGroupName.Unknown;
    public ResourceName ActionName { get; private set; } = ResourceName.Unknown;

    public bool HasAction(string groupName, string actionName) =>
        GroupName.Equals(groupName) && ActionName.Equals(actionName);

    public ScheduledAppAgendaItemActionBuilder Action(IAppApiAction action) => Action(action.Path);

    public ScheduledAppAgendaItemActionBuilder Action(XtiPath path) => Action(path.Group, path.Action);

    public ScheduledAppAgendaItemActionBuilder Action(ResourceGroupName groupName, ResourceName actionName)
    {
        GroupName = groupName;
        ActionName = actionName;
        return new ScheduledAppAgendaItemActionBuilder(this);
    }

    internal ScheduledAppAgendaItemBuilder Disable()
    {
        isEnabled = false;
        return this;
    }

    internal ScheduledAppAgendaItemBuilder RunContinuously()
    {
        type = ScheduledActionTypes.Continuous;
        return this;
    }

    internal ScheduledAppAgendaItemBuilder RunUntilSuccess()
    {
        type = ScheduledActionTypes.PeriodicUntilSuccess;
        return this;
    }

    internal ScheduledAppAgendaItemBuilder AddSchedule(Schedule schedule)
    {
        schedules.Add(schedule);
        return this;
    }

    internal ScheduledAppAgendaItemBuilder DelayAfterStart(TimeSpan delayAfterStart)
    {
        this.delayAfterStart = delayAfterStart;
        return this;
    }

    internal ScheduledAppAgendaItemBuilder Interval(TimeSpan interval)
    {
        this.interval = interval;
        return this;
    }

    internal ScheduledAppAgendaItem Build()
        => new
        (
            GroupName,
            ActionName,
            isEnabled,
            new AggregateSchedule(schedules.ToArray()),
            type,
            delayAfterStart,
            interval
        );

    AppAgendaItem IAppAgendaItemBuilder.Build() => Build();
}
