using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using XTI_App;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_AppTool
{
    public sealed class HostedService : IHostedService
    {
        private readonly IHostApplicationLifetime lifetime;
        private readonly AppFactory appFactory;
        private readonly Clock clock;
        private readonly AppToolOptions appToolOptions;

        public HostedService
        (
            IHostApplicationLifetime lifetime,
            AppFactory appFactory,
            Clock clock,
            IOptions<AppToolOptions> appToolOptions
        )
        {
            this.lifetime = lifetime;
            this.appFactory = appFactory;
            this.clock = clock;
            this.appToolOptions = appToolOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(appToolOptions.AppName))
                {
                    throw new ArgumentException("App name is required");
                }
                if (string.IsNullOrWhiteSpace(appToolOptions.AppType))
                {
                    throw new ArgumentException("App type is required");
                }
                var appType = AppType.Values.Value(appToolOptions.AppType);
                if (appType.Equals(AppType.Values.NotFound))
                {
                    throw new ArgumentException($"App type '{appToolOptions.AppType}' is not valid");
                }
                if (appToolOptions.Command == "add")
                {
                    await new AllAppSetup(appFactory, clock).Run();
                    var appKey = new AppKey(appToolOptions.AppName, appType);
                    var defaultAppSetup = new SingleAppSetup
                    (
                        appFactory,
                        clock,
                        appKey,
                        appToolOptions.AppTitle,
                        new AppRoleName[] { }
                    );
                    await defaultAppSetup.Run();
                }
                else
                {
                    throw new NotSupportedException($"Command '{appToolOptions.Command}' is not supported");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
