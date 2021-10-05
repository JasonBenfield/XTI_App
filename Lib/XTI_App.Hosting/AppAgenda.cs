﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XTI_TempLog;

namespace XTI_App.Hosting
{
    public sealed class AppAgenda
    {
        private readonly IServiceScope scope;
        private TempLogSession session;
        private readonly AppAgendaItem[] items;
        private readonly List<IWorker> workers = new List<IWorker>();

        internal AppAgenda
        (
            IServiceProvider sp,
            AppAgendaItem[] items
        )
        {
            scope = sp.CreateScope();
            this.items = items;
        }

        public async Task Start(CancellationToken stoppingToken)
        {
            var factory = scope.ServiceProvider.GetService<IActionRunnerFactory>();
            session = factory.CreateTempLogSession();
            await session.StartSession();
            startWorkers(stoppingToken);
        }

        public bool IsRunning() => workers.Any(w => !w.HasStopped);

        public async Task Stop()
        {
            while (IsRunning())
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
            var immediateWorker = new ImmediateActionWorker
            (
                scope.ServiceProvider,
                items.OfType<ImmediateAppAgendaItem>().ToArray()
            );
            immediateWorker.StartAsync(stoppingToken);
            workers.Add(immediateWorker);
            var scheduledAppAgendaItems = items.OfType<ScheduledAppAgendaItem>().ToArray();
            foreach (var scheduledItem in scheduledAppAgendaItems.Where(item => item.IsEnabled))
            {
                var worker = new ScheduledActionWorker(scope.ServiceProvider, scheduledItem);
                worker.StartAsync(stoppingToken);
                workers.Add(worker);
            }
        }
    }
}