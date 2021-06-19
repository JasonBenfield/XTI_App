using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using XTI_Tool;
using XTI_VersionToolApi;
using XTI_Version;

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
                var manageVersionCommand = scope.ServiceProvider.GetService<ManageVersionCommand>();
                var options = scope.ServiceProvider.GetService<IOptions<VersionToolOptions>>().Value;
                var version = await manageVersionCommand.Execute(options);
                var output = new VersionToolOutput
                {
                    VersionKey = version.Key().DisplayText,
                    VersionType = version.Type().DisplayText,
                    VersionNumber = version.Version().ToString(),
                    DevVersionNumber = version.NextPatch().ToString()
                };
                new XtiProcessData().Output(output);
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
