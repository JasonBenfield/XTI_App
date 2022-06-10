using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class AppUserName : TextValue, IEquatable<AppUserName>
{
    public static readonly AppUserName Anon = new AppUserName("xti_anon");

    public static AppUserName InstallationUser(string machineName) =>
        new AppUserName($"xti_inst[{machineName}]", $"xti installer [{machineName}]");

    public static AppUserName SystemUser(AppKey appKey, string machineName) =>
        new AppUserName
        (
            $"xti_sys[{appKey.Serialize()}][{machineName}]",
            $"xti system [{appKey.Name.DisplayText} {appKey.Type.DisplayText}] [{machineName}]"
        );

    public AppUserName(string value)
        : this(value, value)
    {
    }

    private AppUserName(string value, string displayText)
        : base(value.Trim().ToLower() ?? "", displayText)
    {
    }

    public bool Equals(AppUserName? other) => _Equals(other);
}