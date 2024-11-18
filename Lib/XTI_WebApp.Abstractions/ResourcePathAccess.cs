namespace XTI_WebApp.Abstractions;

public sealed record ResourcePathAccess(ResourcePath Path, bool HasAccess)
{
    public ResourcePathAccess()
        : this(new(), false)
    {
    }
}
