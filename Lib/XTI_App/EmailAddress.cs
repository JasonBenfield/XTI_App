using System;
using XTI_Core;

namespace XTI_App
{
    public sealed class EmailAddress : TextValue, IEquatable<EmailAddress>
    {
        public EmailAddress(string value)
            : base(value?.Trim().ToLower() ?? "")
        {
        }

        public bool Equals(EmailAddress other) => _Equals(other);
    }
}
