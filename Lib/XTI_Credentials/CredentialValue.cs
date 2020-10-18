using System;

namespace XTI_Credentials
{
    public sealed class CredentialValue : IEquatable<CredentialValue>
    {
        public CredentialValue(string userName, string password)
        {
            UserName = userName;
            Password = password;
            hashCode = $"{UserName}|{Password}".GetHashCode();
        }

        private readonly int hashCode;

        public string UserName { get; }
        public string Password { get; }

        public bool Equals(CredentialValue other)
        {
            return UserName == other.UserName && Password == other.Password;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CredentialValue);
        }

        public override int GetHashCode() => hashCode;

        public override string ToString() => $"{nameof(CredentialValue)} {UserName}";

    }
}
