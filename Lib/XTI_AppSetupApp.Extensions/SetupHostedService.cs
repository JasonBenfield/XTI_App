using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_AppSetupApp.Extensions
{
    public sealed class SetupHostedService : IHostedService
    {
        private readonly IServiceProvider services;

        public SetupHostedService(IServiceProvider services)
        {
            this.services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();
            var setup = scope.ServiceProvider.GetService<IAppSetup>();
            var options = scope.ServiceProvider.GetService<IOptions<SetupOptions>>().Value;
            var versionKey = string.IsNullOrWhiteSpace(options.VersionKey)
                ? AppVersionKey.Current
                : AppVersionKey.Parse(options.VersionKey);
            await setup.Run(versionKey);
            var lifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
            lifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
