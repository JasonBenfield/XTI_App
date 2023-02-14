using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class AppUserName : TextValue, IEquatable<AppUserName>
{
    public static readonly AppUserName Anon = new("xti_anon");

    internal static AppUserName InstallerUser(string machineName) =>
        new($"xti_inst2[{machineName}]", $"xti_inst2[{machineName}]");

    internal static AppUserName SystemUser(AppKey appKey, string machineName) =>
        new
        (
            $"xti_sys2[{appKey.Serialize()}][{machineName}]",
            $"xti_sys2[{appKey.Serialize()}][{machineName}]"
        );

    public AppUserName()
        : this("")
    {
    }

    public AppUserName(string value)
        : this(value, value)
    {
    }

    private AppUserName(string value, string displayText)
        : base(value.Trim().ToLower() ?? "", displayText)
    {
    }

    public bool IsAnon() => Equals(Anon);

    public bool Equals(AppUserName? other) => _Equals(other);
}