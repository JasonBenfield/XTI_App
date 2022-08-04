using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class AppVersionName : TextValue, IEquatable<AppVersionName>
{
    public static readonly AppVersionName None = new AppVersionName();
    public static readonly AppVersionName Unknown = new AppVersionName("Unknown");

    public AppVersionName() : this("") { }

    public AppVersionName(string value) : base(value.ToLower(), value)
    {
    }

    public bool Equals(AppVersionName? other) => _Equals(other);
}
