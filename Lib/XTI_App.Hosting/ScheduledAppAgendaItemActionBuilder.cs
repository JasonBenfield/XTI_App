using XTI_Schedule;

namespace XTI_App.Hosting;

public sealed class ScheduledAppAgendaItemActionBuilder
{
    private readonly ScheduledAppAgendaItemBuilder builder;

    internal ScheduledAppAgendaItemActionBuilder(ScheduledAppAgendaItemBuilder builder)
    {
        this.builder = builder;
    }

    public ScheduledAppAgendaItemActionBuilder Disable()
    {
        builder.Disable();
        return this;
    }

    public ScheduledAppAgendaItemActionBuilder RunContinuously()
    {
        builder.RunContinuously();
        return this;
    }

    public ScheduledAppAgendaItemActionBuilder RunUntilSuccess()
    {
        builder.RunUntilSuccess();
        return this;
    }

    public ScheduledAppAgendaItemActionBuilder AddSchedule(Schedule schedule)
    {
        builder.AddSchedule(schedule);
        return this;
    }

    public ScheduledAppAgendaItemActionBuilder DelayAfterStart(TimeSpan delayAfterStart)
    {
        builder.DelayAfterStart(delayAfterStart);
        return this;
    }

    public ScheduledAppAgendaItemActionBuilder Interval(TimeSpan interval)
    {
        builder.Interval(interval);
        return this;
    }
}