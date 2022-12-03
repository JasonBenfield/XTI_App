using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiUser : IAppApiUser
{
    private readonly CurrentUserAccess currentUserAccess;
    private readonly IXtiPathAccessor xtiPathAccessor;

    public AppApiUser(CurrentUserAccess currentUserAccess, IXtiPathAccessor xtiPathAccessor)
    {
        this.currentUserAccess = currentUserAccess;
        this.xtiPathAccessor = xtiPathAccessor;
    }

    public async Task<bool> HasAccess(XtiPath path)
    {
        path = GetModifiedPath(path);
        var result = await currentUserAccess.HasAccess(path);
        return result.HasAccess;
    }

    public async Task EnsureUserHasAccess(XtiPath path)
    {
        path = GetModifiedPath(path);
        var result = await currentUserAccess.HasAccess(path);
        if (!result.HasAccess)
        {
            throw new AccessDeniedException(result.AccessDeniedMessage);
        }
    }

    private XtiPath GetModifiedPath(XtiPath path)
    {
        var currentPath = xtiPathAccessor.Value();
        if (!path.Action.IsBlank() && !currentPath.Modifier.IsBlank())
        {
            path = path.WithModifier(currentPath.Modifier);
        }
        return path;
    }

}