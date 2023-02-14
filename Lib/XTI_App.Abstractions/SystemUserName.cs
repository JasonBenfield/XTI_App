using System.Text.RegularExpressions;

namespace XTI_App.Abstractions;

public sealed partial class SystemUserName
{
    public static bool CanParse(AppUserName userName) => UserNameRegex().IsMatch(userName.Value);

    public static SystemUserName Parse(AppUserName userName)
    {
        SystemUserName systemUserName;
        if (UserNameRegex().IsMatch(userName.Value))
        {
            var match = UserNameRegex().Match(userName.Value);
            systemUserName = new SystemUserName
            (
                AppKey.Parse(match.Groups["AppKey"].Value),
                match.Groups["MachineName"].Value
            );
        }
        else
        {
            throw new ArgumentException($"'{userName.DisplayText}' is not a system user name");
        }
        return systemUserName;
    }

    public SystemUserName(AppKey appKey, string machineName)
    {
        AppKey = appKey;
        MachineName = machineName;
        UserName = AppUserName.SystemUser(appKey, machineName);
    }

    public AppKey AppKey { get; }
    public string MachineName { get; }
    public AppUserName UserName { get; }

    [GeneratedRegex("^xti_sys(2)?\\[(?<AppKey>[^\\]]+)\\]\\[(?<MachineName>[^\\]]+)\\]$")]
    private static partial Regex UserNameRegex();
}
