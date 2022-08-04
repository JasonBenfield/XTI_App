namespace XTI_App.Api;

public sealed class UserAccessResult
{
    public static UserAccessResult Denied(string message) => new UserAccessResult(false, message);

    public static UserAccessResult Authorized() => new UserAccessResult(true, "");

    private UserAccessResult(bool hasAccess, string accessDeniedMessage)
    {
        HasAccess = hasAccess;
        AccessDeniedMessage = accessDeniedMessage;
    }

    public bool HasAccess { get; }
    public string AccessDeniedMessage { get; }
}
