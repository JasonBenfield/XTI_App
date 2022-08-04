using System.Text.RegularExpressions;

namespace XTI_App.Abstractions;

public sealed class InstallerUserName
{
    private static readonly Regex regex = new Regex("^xti_inst\\[(?<MachineName>[^\\]]+)\\]$");

    public static bool CanParse(AppUserName userName) => regex.IsMatch(userName.Value);

    public static InstallerUserName Parse(AppUserName userName)
    {
        InstallerUserName installerUserName;
        if (regex.IsMatch(userName.Value))
        {
            var match = regex.Match(userName.Value);
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
        Value = AppUserName.InstallerUser(machineName);
    }

    public string MachineName { get; }
    public AppUserName Value { get; }
}
