using Microsoft.AspNetCore.DataProtection;

namespace XTI_Secrets
{
    public sealed class FileSecretCredentialsFactory : SecretCredentialsFactory
    {
        private readonly string environmentName;

        public FileSecretCredentialsFactory(string environmentName, IDataProtector dataProtector) : base(dataProtector)
        {
            this.environmentName = environmentName;
        }

        protected override SecretCredentials _Create(string key, IDataProtector dataProtector) =>
            new FileSecretCredentials(environmentName, key, dataProtector);
    }
}
