using XTI_Schedule;

namespace XTI_App.Api;

public sealed class ActionScheduleBuilder<TActionBuilder> where TActionBuilder : IAppApiActionBuilder
{
    private readonly TActionBuilder actionBuilder;
    private readonly bool isDisabled;
    private readonly List<Schedule> schedules = new();
    private ScheduledActionTypes type = ScheduledActionTypes.Continuous;
    private TimeSpan delayAfterStart = new();
    private TimeSpan interval = TimeSpan.FromDays(1);

    public ActionScheduleBuilder(TActionBuilder actionBuilder)
    {
        this.actionBuilder = actionBuilder;
        isDisabled = true;
    }

    public ActionScheduleBuilder(TActionBuilder actionBuilder, ScheduledActionTypes type)
    {
        this.actionBuilder = actionBuilder;
        this.type = type;
        isDisabled = false;
    }

    public ActionScheduleBuilder<TActionBuilder> DelayAfterStart(TimeSpan delayAfterStart)
    {
        this.delayAfterStart = delayAfterStart;
        return this;
    }

    public ActionScheduleBuilder<TActionBuilder> Interval(TimeSpan interval)
    {
        this.interval = interval;
        return this;
    }

    public TActionBuilder AddSchedule(Schedule schedule) =>
        AddSchedules(schedule);

    public TActionBuilder AddSchedules(params Schedule[] schedules)
    {
        this.schedules.AddRange(schedules);
        return actionBuilder;
    }

    public ScheduledAppAgendaItemOptions Build()
    {
        var actionPath = actionBuilder.ActionPath();
        return new ScheduledAppAgendaItemOptions
        {
            GroupName = actionPath.Group.DisplayText,
            ActionName = actionPath.Action.DisplayText,
            IsDisabled = isDisabled,
            Schedule = new AggregateSchedule(schedules.ToArray()).ToScheduleOptions(),
            Type = type,
            DelayAfterStart = delayAfterStart,
            Interval = interval
        };
    }
}
