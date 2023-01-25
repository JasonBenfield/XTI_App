namespace XTI_App.Abstractions;

public sealed class AccessDeniedException : AppException
{
    public AccessDeniedException(string message)
        : base(message, "Access Denied")
    {
    }
}