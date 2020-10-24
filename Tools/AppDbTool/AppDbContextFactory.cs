using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.DB;
using XTI_App.Extensions;
using XTI_Configuration.Extensions;

namespace AppDbApp
{
    public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.UseXtiConfiguration(hostingContext.HostingEnvironment.EnvironmentName, args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<AppDbAppOptions>(hostContext.Configuration);
                    services.AddAppDbContextForSqlServer(hostContext.Configuration);
                })
                .Build();
            var scope = host.Services.CreateScope();
            return scope.ServiceProvider.GetService<AppDbContext>();
        }
    }
}
