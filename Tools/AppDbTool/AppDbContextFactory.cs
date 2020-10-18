using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using XTI_App.DB;
using XTI_App.EF;
using XTI_Configuration.Extensions;

namespace AppDbApp
{
    public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            }
            configBuilder.UseXtiConfiguration(environment, args);
            var config = configBuilder.Build();
            var section = config.GetSection(DbOptions.DB);
            var appDbOptions = section.Get<DbOptions>();
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer
                (
                    new AppConnectionString(appDbOptions, environment).Value(),
                    b => b.MigrationsAssembly("AppDbTool")
                )
                .EnableSensitiveDataLogging();
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
