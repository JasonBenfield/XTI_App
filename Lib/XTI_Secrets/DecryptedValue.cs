using Microsoft.AspNetCore.DataProtection;
using System;
using System.Text;

namespace XTI_Secrets
{
    public sealed class DecryptedValue
    {
        private readonly IDataProtector dataProtector;
        private readonly string encryptedText;

        public DecryptedValue(IDataProtector dataProtector, string encryptedText)
        {
            this.dataProtector = dataProtector;
            this.encryptedText = encryptedText;
        }

        public string Value()
        {
            var protectedBytes = Convert.FromBase64String(encryptedText);
            var unprotectedBytes = dataProtector.Unprotect(protectedBytes);
            return UTF8Encoding.UTF8.GetString(unprotectedBytes);
        }
    }
}
