using System;
using System.Security.Cryptography;

namespace XTI_App
{
    public sealed class Md5HashedPassword : HashedPassword
    {
        internal Md5HashedPassword(string password) : base(password)
        {
        }

        private const int saltLength = 16;
        private const int hashLength = 20;

        protected override string Hash(string password)
        {
            var salt = new byte[saltLength];
            new RNGCryptoServiceProvider().GetBytes(salt);
            return hash(password, salt);
        }

        protected override bool _Equals(string password, string other)
        {
            var otherHashedBytes = Convert.FromBase64String(other);
            var salt = new byte[16];
            Array.Copy(otherHashedBytes, 0, salt, 0, 16);
            var thisHashed = hash(password, salt);
            return other == thisHashed;
        }

        private static string hash(string password, byte[] salt)
        {
            var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100000);
            var hash = deriveBytes.GetBytes(hashLength);
            var hashBytes = new byte[saltLength + hashLength];
            Array.Copy(salt, 0, hashBytes, 0, saltLength);
            Array.Copy(hash, 0, hashBytes, saltLength, hashLength);
            return Convert.ToBase64String(hashBytes);
        }

    }
}
