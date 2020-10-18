using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using XTI_Configuration.Extensions;
using XTI_Tool.Extensions;
using XTI_Version;

namespace XTI_VersionTool
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
                    services.Configure<ManageVersionOptions>(hostContext.Configuration);
                    services.AddScoped<ManageVersionCommand>();
                    services.AddHostedService(sp =>
                    {
                        var scope = sp.CreateScope();
                        var lifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
                        var manageVersionCommand = scope.ServiceProvider.GetService<ManageVersionCommand>();
                        var options = scope.ServiceProvider.GetService<IOptions<ManageVersionOptions>>();
                        return new VersionManager(lifetime, manageVersionCommand, options);
                    });
                })
                .RunConsoleAsync();
        }
    }
}
