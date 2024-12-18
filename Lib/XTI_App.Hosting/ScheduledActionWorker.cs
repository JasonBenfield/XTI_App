using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;

namespace XTI_App.Hosting;

public sealed class ScheduledActionWorker : BackgroundService, IWorker
{
    private readonly IServiceProvider sp;
    private readonly IClock clock;
    private readonly XtiEnvironment environment;
    private readonly XtiBasePath xtiBasePath;
    private readonly ScheduledAppAgendaItem scheduledItem;

    public ScheduledActionWorker
    (
        IServiceProvider sp,
        IClock clock,
        XtiEnvironment environment,
        XtiBasePath xtiBasePath,
        ScheduledAppAgendaItem scheduledItem
    )
    {
        this.sp = sp;
        this.clock = clock;
        this.environment = environment;
        this.xtiBasePath = xtiBasePath;
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
            var schedule = scheduledItem.Schedule;
            if (schedule.IsInSchedule(clock.Now()))
            {
                if (scheduledItem.Type != ScheduledActionTypes.PeriodicUntilSuccess || !periodicSucceeded)
                {
                    var actionRunner = new ActionRunner
                    (
                        sp,
                        environment,
                        xtiBasePath,
                        scheduledItem.GroupName.DisplayText,
                        scheduledItem.ActionName.DisplayText
                    );
                    var result = await actionRunner.Run(stoppingToken);
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