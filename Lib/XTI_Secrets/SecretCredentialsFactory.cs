using Microsoft.AspNetCore.DataProtection;

namespace XTI_Secrets
{
    public abstract class SecretCredentialsFactory
    {
        private readonly IDataProtector dataProtector;

        protected SecretCredentialsFactory(IDataProtector dataProtector)
        {
            this.dataProtector = dataProtector;
        }

        public SecretCredentials Create(string key) => _Create(key, dataProtector);

        protected abstract SecretCredentials _Create(string key, IDataProtector dataProtector);
    }
}
