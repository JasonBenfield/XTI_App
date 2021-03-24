using System;
using XTI_Core;

namespace XTI_App.Abstractions
{
    public sealed class AppRoleName : TextKeyValue, IEquatable<AppRoleName>
    {
        public AppRoleName(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(value)} is required");
            }
        }

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public bool Equals(AppRoleName other) => _Equals(other);
    }
}
