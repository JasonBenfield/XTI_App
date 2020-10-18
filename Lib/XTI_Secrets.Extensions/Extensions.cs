using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace XTI_Secrets.Extensions
{
    public static class Extensions
    {
        public static void AddDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            var secretOptions = configuration.GetSection(SecretOptions.Secret).Get<SecretOptions>();
            services
                .AddDataProtection
                (
                    options => options.ApplicationDiscriminator = secretOptions.ApplicationName
                )
                .PersistKeysToFileSystem(new DirectoryInfo(secretOptions.KeyDirectoryPath))
                .SetApplicationName(secretOptions.ApplicationName);
        }

        public static void AddFileSecretCredentials(this IServiceCollection services)
        {
            services.AddScoped<SecretCredentialsFactory>(sp =>
            {
                var hostEnv = sp.GetService<IHostEnvironment>();
                var dataProtector = sp.GetDataProtector(new[] { "XTI_Secrets" });
                return new FileSecretCredentialsFactory(hostEnv.EnvironmentName, dataProtector);
            });
        }
    }
}
