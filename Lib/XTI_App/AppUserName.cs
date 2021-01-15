using System;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppUserName : TextValue, IEquatable<AppUserName>
    {
        public static readonly AppUserName Anon = new AppUserName("xti_anon");
        public static readonly AppUserName SuperUser = new AppUserName("xti_superuser");

        public AppUserName(string value) : base(value?.Trim().ToLower() ?? "")
        {
        }

        public override bool Equals(object obj) => base.Equals(obj);

        public bool Equals(AppUserName other) => _Equals(other);

        public override int GetHashCode() => base.GetHashCode();

    }
}
