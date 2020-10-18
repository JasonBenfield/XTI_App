using System;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppKey : SemanticType<string>, IEquatable<AppKey>
    {
        public AppKey(string value)
            : base(value?.Trim().ToLower() ?? "", value)
        {
        }

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public bool Equals(AppKey other) => _Equals(other);
    }
}
