using System;
using XTI_Core;

namespace XTI_App.Abstractions
{
    public sealed class AppRoleName : TextKeyValue, IEquatable<AppRoleName>
    {
        public static AppRoleName General = new AppRoleName(nameof(General));
        public static AppRoleName Admin = new AppRoleName(nameof(Admin));
        public static AppRoleName System = new AppRoleName(nameof(System));
        public static AppRoleName ManageUserCache = new AppRoleName(nameof(ManageUserCache));

        public static AppRoleName[] DefaultRoles() => new[] { General, Admin, System, ManageUserCache };

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
