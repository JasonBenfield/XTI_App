using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class AppRoleName : TextKeyValue, IEquatable<AppRoleName>
{
    public static AppRoleName Admin = new AppRoleName(nameof(Admin));
    public static AppRoleName System = new AppRoleName(nameof(System));
    public static AppRoleName ManageUserCache = new AppRoleName(nameof(ManageUserCache));
    public static AppRoleName DenyAccess = new AppRoleName(nameof(DenyAccess));

    public static AppRoleName[] DefaultRoles() => new[] 
    {
        Admin, 
        System, 
        ManageUserCache, 
        DenyAccess
    };

    public AppRoleName(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{nameof(value)} is required");
        }
    }

    public bool Equals(AppRoleName? other) => _Equals(other);
    public bool EqualsAny(params AppRoleName[] others) => _EqualsAny(others);
}