using System.Text.RegularExpressions;

namespace XTI_App.Abstractions;

public sealed class SystemUserName
{
    private static readonly Regex regex = new Regex("^xti_sys\\[(?<AppKey>[^\\]]+)\\]\\[(?<MachineName>[^\\]]+)\\]$");

    public static bool CanParse(AppUserName userName) => regex.IsMatch(userName.Value);

    public static SystemUserName Parse(AppUserName userName)
    {
        SystemUserName systemUserName;
        if (regex.IsMatch(userName.Value))
        {
            var match = regex.Match(userName.Value);
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
        Value = AppUserName.SystemUser(appKey, machineName);
    }

    public AppKey AppKey { get; }
    public string MachineName { get; }
    public AppUserName Value { get; }
}
