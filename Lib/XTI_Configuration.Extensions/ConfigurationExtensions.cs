using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace XTI_Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void UseXtiConfiguration(this IConfigurationBuilder config, string envName, string[] args)
        {
            config.Sources.Clear();
            var sharedDir = Path.Combine
            (
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "XTI",
                "Shared"
            );
            config
                .AddJsonFile(Path.Combine(sharedDir, "appsettings.json"), optional: true, reloadOnChange: true)
                .AddJsonFile(Path.Combine(sharedDir, $"appsettings.{envName}.json"),
                                 optional: true, reloadOnChange: true)
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{envName}.json",
                                 optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            if (args != null)
            {
                config.AddCommandLine(args);
            }
        }
    }
}
