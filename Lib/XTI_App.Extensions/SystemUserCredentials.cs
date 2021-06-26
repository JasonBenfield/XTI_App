using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_Credentials;
using XTI_Secrets;

namespace XTI_App.Extensions
{
    public sealed class SystemUserCredentials
    {
        private readonly ISecretCredentialsFactory secretCredentialsFactory;
        private readonly AppKey appKey;

        public SystemUserCredentials(ISecretCredentialsFactory secretCredentialsFactory, AppKey appKey)
        {
            this.secretCredentialsFactory = secretCredentialsFactory;
            this.appKey = appKey;
        }

        public Task<CredentialValue> Value()
        {
            var secretCredentials = secretCredentialsFactory.Create(getCredentialKey());
            return secretCredentials.Value();
        }

        public Task Update(CredentialValue credentials)
        {
            var secretCredentials = secretCredentialsFactory.Create(getCredentialKey());
            return secretCredentials.Update(credentials);
        }

        private string getCredentialKey()
            => $"System_User_{appKey.Type.DisplayText}_{appKey.Name.DisplayText}"
                .Replace(" ", "");

    }
}
