using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using XTI_App.DB;
using XTI_App.EF;
using XTI_Tool.Extensions;

namespace AppDbApp
{
    public sealed class HostedService : IHostedService
    {
        private readonly IHostApplicationLifetime lifetime;
        private readonly IHostEnvironment hostEnvironment;
        private readonly AppDbReset appDbReset;
        private readonly AppDbBackup appDbBackup;
        private readonly AppDbRestore appDbRestore;
        private readonly AppDbAppOptions options;

        public HostedService
        (
            IHostApplicationLifetime lifetime,
            IHostEnvironment hostEnvironment,
            AppDbReset appDbReset,
            AppDbBackup appDbBackup,
            AppDbRestore appDbRestore,
            IOptions<AppDbAppOptions> options
        )
        {
            this.lifetime = lifetime;
            this.hostEnvironment = hostEnvironment;
            this.appDbReset = appDbReset;
            this.appDbBackup = appDbBackup;
            this.appDbRestore = appDbRestore;
            this.options = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (options.Command == "reset")
                {
                    if (!hostEnvironment.IsTest() && !options.Force)
                    {
                        throw new ArgumentException("Database reset can only be run for the test environment");
                    }
                    await appDbReset.Run();
                }
                else if (options.Command == "backup")
                {
                    if (string.IsNullOrWhiteSpace(options.BackupFilePath))
                    {
                        throw new ArgumentException("Backup file path is required for backup");
                    }
                    await appDbBackup.Run(hostEnvironment.EnvironmentName, options.BackupFilePath);
                }
                else if (options.Command == "restore")
                {
                    if (hostEnvironment.IsProduction())
                    {
                        throw new ArgumentException("Database restore cannot be run for the production environment");
                    }
                    if (string.IsNullOrWhiteSpace(options.BackupFilePath))
                    {
                        throw new ArgumentException("Backup file path is required for restore");
                    }
                    await appDbRestore.Run(hostEnvironment.EnvironmentName, options.BackupFilePath);
                }
                else
                {
                    throw new NotSupportedException($"Command '{options.Command}' is not supported");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Environment.ExitCode = 999;
            }
            lifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
