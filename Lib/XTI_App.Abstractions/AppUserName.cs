using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class AppUserName : TextValue, IEquatable<AppUserName>
{
    public static readonly AppUserName Anon = new AppUserName("xti_anon");

    public static AppUserName SystemUser(AppKey appKey, string machineName)
    {
        var parts = new[]
        {
                "xti",
                "sys",
                appKey.Name.DisplayText,
                appKey.Type.DisplayText,
                machineName
            }
        .Where(p => !string.IsNullOrWhiteSpace(p))
        .Select(p => p.Replace(" ", "").Replace("_", ""));
        var userName = string.Join("_", parts);
        return new AppUserName(userName);
    }

    public AppUserName(string value) : base(value?.Trim().ToLower() ?? "")
    {
    }

    public bool Equals(AppUserName? other) => _Equals(other);
}