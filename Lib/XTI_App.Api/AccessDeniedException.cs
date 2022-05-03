namespace XTI_App.Api;

public sealed class AccessDeniedException : AppException
{
    public AccessDeniedException(string message)
        : base(message, "Access Denied")
    {
    }
}