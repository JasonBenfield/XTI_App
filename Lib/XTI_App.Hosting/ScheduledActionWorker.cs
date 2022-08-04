using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_Core;

namespace XTI_App.Hosting;

public sealed class ScheduledActionWorker : BackgroundService, IWorker
{
    private readonly IServiceProvider sp;
    private readonly ScheduledAppAgendaItem scheduledItem;

    public ScheduledActionWorker(IServiceProvider sp, ScheduledAppAgendaItem scheduledItem)
    {
        this.sp = sp;
        this.scheduledItem = scheduledItem;
    }

    public bool HasStopped { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var periodicSucceeded = false;
        try
        {
            await Task.Delay(scheduledItem.DelayAfterStart, stoppingToken);
        }
        catch (TaskCanceledException) { }
        while (!stoppingToken.IsCancellationRequested)
        {
            var clock = sp.GetRequiredService<IClock>();
            var schedule = scheduledItem.Schedule;
            if (schedule.IsInSchedule(clock.Now()))
            {
                if (scheduledItem.Type != ScheduledActionTypes.PeriodicUntilSuccess || !periodicSucceeded)
                {
                    var actionExecutor = new ActionRunner
                    (
                        sp,
                        scheduledItem.GroupName.DisplayText,
                        scheduledItem.ActionName.DisplayText
                    );
                    var result = await actionExecutor.Run(stoppingToken);
                    if (result == ActionRunner.Results.Succeeded)
                    {
                        periodicSucceeded = true;
                    }
                }
            }
            else
            {
                periodicSucceeded = false;
            }
            try
            {
                await Task.Delay(scheduledItem.Interval, stoppingToken);
            }
            catch (TaskCanceledException) { }
        }
        HasStopped = true;
    }
}