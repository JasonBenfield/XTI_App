using System;
using XTI_Core;

namespace XTI_App.Abstractions
{
    public sealed class AppName : TextKeyValue, IEquatable<AppName>
    {
        public static readonly AppName Unknown = new AppName("Unknown");

        public AppName(string value) : base(value)
        {
        }

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public bool Equals(AppName other) => _Equals(other);
    }
}
