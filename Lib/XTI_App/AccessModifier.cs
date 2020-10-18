using System;
using XTI_Core;

namespace XTI_App
{
    public sealed class AccessModifier : SemanticType<string>, IEquatable<AccessModifier>
    {
        public static bool operator ==(AccessModifier a, AccessModifier b) => Equals(a, b);
        public static bool operator !=(AccessModifier a, AccessModifier b) => !(a == b);

        public static readonly AccessModifier Default = new AccessModifier("");

        public static AccessModifier FromValue(string value) =>
            string.IsNullOrWhiteSpace(value) ? Default : new AccessModifier(value);

        public AccessModifier(string value)
            : base(value?.Trim(), value)
        {
        }

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public bool Equals(AccessModifier other) => _Equals(other);
    }
}
