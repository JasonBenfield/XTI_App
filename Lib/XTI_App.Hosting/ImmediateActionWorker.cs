using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using XTI_App.Api;
using XTI_Schedule;

namespace XTI_App.Hosting
{
    public sealed class ImmediateActionWorker : BackgroundService, IWorker
    {
        private readonly IServiceProvider sp;
        private readonly ImmediateActionOptions[] optionsArray;

        public ImmediateActionWorker(IServiceProvider sp, ImmediateActionOptions[] optionsArray)
        {
            this.sp = sp;
            this.optionsArray = optionsArray;
        }

        public bool HasStopped { get; private set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var options in optionsArray)
            {
                var actionExecutor = new ActionRunner
                (
                    sp,
                    options.GroupName,
                    options.ActionName
                );
                await actionExecutor.Run();
            }
            HasStopped = true;
        }
    }
}
