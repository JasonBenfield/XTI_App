using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using XTI_App;
using XTI_Configuration.Extensions;
using XTI_Tool.Extensions;
using XTI_Core;
using XTI_Secrets;
using XTI_Secrets.Extensions;

namespace XTI_UserApp
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
                    services.AddFileSecretCredentials();
                    services.AddScoped<IHashedPasswordFactory, Md5HashedPasswordFactory>();
                    services.Configure<UserOptions>(hostContext.Configuration);
                    services.AddHostedService(sp =>
                    {
                        var scope = sp.CreateScope();
                        var lifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
                        var appFactory = scope.ServiceProvider.GetService<AppFactory>();
                        var hashedPasswordFactory = scope.ServiceProvider.GetService<IHashedPasswordFactory>();
                        var secretCredentialsFactory = scope.ServiceProvider.GetService<SecretCredentialsFactory>();
                        var clock = scope.ServiceProvider.GetService<Clock>();
                        var userOptions = scope.ServiceProvider.GetService<IOptions<UserOptions>>();
                        return new HostedService
                        (
                            lifetime,
                            appFactory,
                            hashedPasswordFactory,
                            secretCredentialsFactory,
                            clock,
                            userOptions
                        );
                    });
                })
                .RunConsoleAsync();
        }
    }
}
