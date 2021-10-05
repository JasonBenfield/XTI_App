using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using XTI_Core;

namespace XTI_App.Hosting
{
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
            await Task.Delay(scheduledItem.DelayAfterStart, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                var clock = sp.GetService<Clock>();
                var schedule = scheduledItem.Schedule;
                if (schedule.IsInSchedule(clock.Now()))
                {
                    if (scheduledItem.Type != ScheduledActionTypes.PeriodicUntilSuccess || !periodicSucceeded)
                    {
                        var actionExecutor = new ActionRunner
                        (
                            sp,
                            scheduledItem.GroupName,
                            scheduledItem.ActionName
                        );
                        var result = await actionExecutor.Run();
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
                await Task.Delay(scheduledItem.Interval, stoppingToken);
            }
            HasStopped = true;
        }
    }
}
