using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiUser : IAppApiUser
{
    private readonly CurrentUserAccess currentUserAccess;
    private readonly IModifierKeyAccessor modifierKeyAccessor;

    public AppApiUser(CurrentUserAccess currentUserAccess, IModifierKeyAccessor modifierKeyAccessor)
    {
        this.currentUserAccess = currentUserAccess;
        this.modifierKeyAccessor = modifierKeyAccessor;
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
        var modKey = modifierKeyAccessor.Value();
        if (!path.Action.IsBlank() && !modKey.IsBlank())
        {
            path = path.WithModifier(modKey);
        }
        return path;
    }

}