using Microsoft.AspNetCore.DataProtection;
using System.Collections.Generic;

namespace XTI_Secrets.Fakes
{
    public sealed class FakeSecretCredentialsFactory : SecretCredentialsFactory
    {
        public FakeSecretCredentialsFactory(IDataProtector dataProtector)
            : base(dataProtector)
        {
        }

        private readonly Dictionary<string, FakeSecretCredentials> credentialLookup = new Dictionary<string, FakeSecretCredentials>();

        protected override SecretCredentials _Create(string key, IDataProtector dataProtector)
        {
            if (!credentialLookup.TryGetValue(key, out var secretCredentials))
            {
                secretCredentials = new FakeSecretCredentials(key, dataProtector);
                credentialLookup.Add(key, secretCredentials);
            }
            return secretCredentials;
        }
    }
}
