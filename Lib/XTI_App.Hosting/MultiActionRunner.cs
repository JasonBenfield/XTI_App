using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XTI_Schedule;
using XTI_TempLog;

namespace XTI_App.Hosting
{
    public sealed class MultiActionRunner
    {
        private readonly IServiceScope scope;
        private TempLogSession session;
        private readonly ImmediateActionOptions[] immediateActions;
        private readonly ScheduledActionOptions[] scheduledActions;
        private readonly AlwaysRunningActionOptions[] alwaysRunningActions;
        private readonly List<IWorker> workers = new List<IWorker>();

        public MultiActionRunner
        (
            IServiceProvider sp,
            ImmediateActionOptions[] immediateActions,
            ScheduledActionOptions[] scheduledActions,
            AlwaysRunningActionOptions[] alwaysRunningActions
        )
        {
            scope = sp.CreateScope();
            this.immediateActions = immediateActions;
            this.scheduledActions = scheduledActions;
            this.alwaysRunningActions = alwaysRunningActions;
        }

        public async Task Start(CancellationToken stoppingToken)
        {
            var factory = scope.ServiceProvider.GetService<IActionRunnerFactory>();
            session = factory.CreateTempLogSession();
            await session.StartSession();
            startWorkers(stoppingToken);
        }

        public async Task Stop()
        {
            while (workers.Any(w => !w.HasStopped))
            {
                await Task.Delay(100);
            }
            if (session != null)
            {
                await session.EndSession();
            }
        }

        private void startWorkers(CancellationToken stoppingToken)
        {
            var immediateWorker = new ImmediateActionWorker(scope.ServiceProvider, immediateActions);
            immediateWorker.StartAsync(stoppingToken);
            workers.Add(immediateWorker);
            foreach (var alwaysRunningActionOptions in alwaysRunningActions)
            {
                var scheduledActionOptions = new ScheduledActionOptions
                {
                    GroupName = alwaysRunningActionOptions.GroupName,
                    ActionName = alwaysRunningActionOptions.ActionName,
                    Interval = alwaysRunningActionOptions.Interval,
                    Schedule = new ScheduleOptions
                    {
                        WeeklyTimeRanges = new[]
                        {
                            new WeeklyTimeRangeOptions
                            {
                                DaysOfWeek = new[]
                                {
                                    DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday
                                },
                                TimeRanges = new [] { new TimeRangeOptions { StartTime = 0, EndTime = 2400 } }
                            }
                        }
                    }
                };
                var worker = new ScheduledActionWorker(scope.ServiceProvider, scheduledActionOptions);
                worker.StartAsync(stoppingToken);
                workers.Add(worker);
            }
            foreach (var scheduledActionOptions in scheduledActions)
            {
                var worker = new ScheduledActionWorker(scope.ServiceProvider, scheduledActionOptions);
                worker.StartAsync(stoppingToken);
                workers.Add(worker);
            }
        }
    }
}
