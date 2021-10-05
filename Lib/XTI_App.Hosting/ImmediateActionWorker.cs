using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XTI_App.Hosting
{
    public sealed class ImmediateActionWorker : BackgroundService, IWorker
    {
        private readonly IServiceProvider sp;
        private readonly ImmediateAppAgendaItem[] agendaItems;

        public ImmediateActionWorker(IServiceProvider sp, ImmediateAppAgendaItem[] agendaItems)
        {
            this.sp = sp;
            this.agendaItems = agendaItems;
        }

        public bool HasStopped { get; private set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var agendaItem in agendaItems.Where(item => item.IsEnabled))
            {
                var actionExecutor = new ActionRunner
                (
                    sp,
                    agendaItem.GroupName,
                    agendaItem.ActionName
                );
                await actionExecutor.Run();
            }
            HasStopped = true;
        }
    }
}
