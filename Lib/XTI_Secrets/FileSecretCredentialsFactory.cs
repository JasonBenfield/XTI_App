using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Hosting;

namespace XTI_Secrets
{
    public sealed class FileSecretCredentialsFactory : SecretCredentialsFactory
    {
        private readonly IHostEnvironment hostEnv;

        public FileSecretCredentialsFactory(IHostEnvironment hostEnv, IDataProtector dataProtector) : base(dataProtector)
        {
            this.hostEnv = hostEnv;
        }

        protected override SecretCredentials _Create(string key, IDataProtector dataProtector) =>
            new FileSecretCredentials(hostEnv, key, dataProtector);
    }
}
