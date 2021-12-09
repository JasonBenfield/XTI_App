using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AccessDeniedException : AppException
{
    public AccessDeniedException(XtiPath resourceName)
        : base($"Access denied to {resourceName.Format()}", "Access Denied")
    {
        ResourceName = resourceName;
    }

    public XtiPath ResourceName { get; }
}