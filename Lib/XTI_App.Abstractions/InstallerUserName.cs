using System.Text.RegularExpressions;

namespace XTI_App.Abstractions;

public sealed partial class InstallerUserName
{
    public static bool CanParse(AppUserName userName) => UserNameRegex().IsMatch(userName.Value);

    public static InstallerUserName Parse(AppUserName userName)
    {
        InstallerUserName installerUserName;
        if (UserNameRegex().IsMatch(userName.Value))
        {
            var match = UserNameRegex().Match(userName.Value);
            installerUserName = new InstallerUserName(match.Groups["MachineName"].Value);
        }
        else
        {
            throw new ArgumentException($"'{userName.DisplayText}' is not an installer user name");
        }
        return installerUserName;
    }

    public InstallerUserName(string machineName)
    {
        MachineName = machineName;
        UserName = AppUserName.InstallerUser(machineName);
    }

    public string MachineName { get; }
    public AppUserName UserName { get; }

    [GeneratedRegex("^xti_inst(2)?\\[(?<MachineName>[^\\]]+)\\]$")]
    private static partial Regex UserNameRegex();
}
