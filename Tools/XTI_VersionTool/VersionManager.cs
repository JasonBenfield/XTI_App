using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using XTI_Version;
using XTI_VersionToolApi;

namespace XTI_VersionTool
{
    public sealed class VersionManager : IHostedService
    {
        private readonly IServiceProvider services;

        public VersionManager(IServiceProvider services)
        {
            this.services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();
            try
            {
                var commandFactory = scope.ServiceProvider.GetService<VersionCommandFactory>();
                var options = scope.ServiceProvider.GetService<IOptions<VersionToolOptions>>().Value;
                var commandName = VersionCommandName.FromValue(options.Command);
                var command = commandFactory.Create(commandName);
                await command.Execute(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Environment.ExitCode = 999;
            }
            var lifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
            lifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
