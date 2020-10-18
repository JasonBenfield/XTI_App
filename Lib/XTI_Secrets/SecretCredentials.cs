using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;
using System.Threading.Tasks;
using XTI_Credentials;

namespace XTI_Secrets
{
    public abstract class SecretCredentials : ICredentials
    {
        private readonly string key;
        private readonly IDataProtector dataProtector;

        protected SecretCredentials(string key, IDataProtector dataProtector)
        {
            this.key = key;
            this.dataProtector = dataProtector;
        }

        public Task Update(string userName, string password) =>
            Update(new CredentialValue(userName, password));

        public Task Update(CredentialValue value)
        {
            var serialized = JsonSerializer.Serialize(new CredentialValueRecord(value));
            var encrypted = new EncryptedValue(dataProtector, serialized).Value();
            return Persist(key, encrypted);
        }

        protected abstract Task Persist(string key, string encryptedText);

        public async Task<CredentialValue> Value()
        {
            var encrypted = await Load(key);
            var serialized = new DecryptedValue(dataProtector, encrypted).Value();
            var deserialized = string.IsNullOrWhiteSpace(serialized)
                ? new CredentialValueRecord()
                : JsonSerializer.Deserialize<CredentialValueRecord>(serialized);
            return new CredentialValue(deserialized.UserName, deserialized.Password);
        }

        protected abstract Task<string> Load(string key);

        private class CredentialValueRecord
        {
            public CredentialValueRecord()
            {
            }

            public CredentialValueRecord(CredentialValue credentialValue)
            {
                UserName = credentialValue?.UserName ?? "";
                Password = credentialValue?.Password ?? "";
            }

            public string UserName { get; set; } = "";
            public string Password { get; set; } = "";
        }
    }
}
