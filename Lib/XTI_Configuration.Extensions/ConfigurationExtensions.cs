using Microsoft.Extensions.Configuration;
using System;
using XTI_Core;

namespace XTI_Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void UseXtiConfiguration(this IConfigurationBuilder config, string envName, string[] args)
        {
            config.Sources.Clear();
            var appDataFolder = new AppDataFolder().WithSubFolder("Shared");
            config
                .AddJsonFile
                (
                    appDataFolder.FilePath("appsettings.json"),
                    optional: true,
                    reloadOnChange: true
                )
                .AddJsonFile
                (
                    appDataFolder.FilePath($"appsettings.{envName}.json"),
                    optional: true,
                    reloadOnChange: true
                )
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile
                (
                    $"appsettings.{envName}.json",
                    optional: true,
                    reloadOnChange: true
                )
                .AddEnvironmentVariables();
            if (args != null)
            {
                config.AddCommandLine(args);
            }
        }
    }
}
