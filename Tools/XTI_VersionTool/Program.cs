using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using XTI_Configuration.Extensions;
using XTI_Tool.Extensions;
using XTI_Version;
using XTI_VersionToolApi;
using XTI_Secrets.Extensions;

namespace XTI_VersionTool
{
    class Program
    {
        static Task Main(string[] args)
           => Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.UseXtiConfiguration(hostingContext.HostingEnvironment, args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddConsoleAppServices(hostContext.HostingEnvironment, hostContext.Configuration);
                    services.AddFileSecretCredentials();
                    services.Configure<VersionToolOptions>(hostContext.Configuration);
                    services.AddScoped<GitFactory, DefaultGitFactory>();
                    services.AddScoped<VersionCommandFactory>();
                    services.AddHostedService<VersionManager>();
                })
                .RunConsoleAsync();
    }
}
