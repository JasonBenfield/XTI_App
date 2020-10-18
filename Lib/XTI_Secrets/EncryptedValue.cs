using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Text;

namespace XTI_Secrets
{
    public sealed class EncryptedValue
    {
        private readonly string encrypted;

        public EncryptedValue(IDataProtector dataProtector, string plainText)
        {
            var unprotectedBytes = Encoding.Default.GetBytes(plainText);
            var protectedBytes = dataProtector.Protect(unprotectedBytes);
            encrypted = Convert.ToBase64String(protectedBytes);
        }

        public string Value() => encrypted;
    }
}
