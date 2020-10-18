using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using XTI_App;
using XTI_Configuration.Extensions;
using XTI_Tool.Extensions;
using XTI_Core;

namespace XTI_AppTool
{
    class Program
    {
        static Task Main(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.UseXtiConfiguration(hostingContext.HostingEnvironment.EnvironmentName, args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddConsoleAppServices(hostContext.Configuration);
                    services.Configure<AppToolOptions>(hostContext.Configuration);
                    services.AddHostedService(sp =>
                    {
                        var scope = sp.CreateScope();
                        var lifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
                        var appFactory = scope.ServiceProvider.GetService<AppFactory>();
                        var clock = scope.ServiceProvider.GetService<Clock>();
                        var appToolOptions = scope.ServiceProvider.GetService<IOptions<AppToolOptions>>();
                        return new HostedService
                        (
                            lifetime,
                            appFactory,
                            clock,
                            appToolOptions
                        );
                    });
                })
                .RunConsoleAsync();
        }
    }
}
