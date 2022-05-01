using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiUser : IAppApiUser
{
    private readonly IAppContext appContext;
    private readonly IUserContext userContext;
    private readonly ICurrentUserName currentUserName;
    private readonly IXtiPathAccessor pathAccessor;

    public AppApiUser(IAppContext appContext, IUserContext userContext, ICurrentUserName currentUserName, IXtiPathAccessor pathAccessor)
    {
        this.appContext = appContext;
        this.userContext = userContext;
        this.currentUserName = currentUserName;
        this.pathAccessor = pathAccessor;
    }

    public async Task<bool> HasAccess(ResourceAccess resourceAccess)
    {
        var app = await appContext.App();
        var roles = await app.Roles();
        var allowedRoleIDs = resourceAccess.Allowed
            .Select(ar => roles.FirstOrDefault(r => r.Name().Equals(ar)))
            .Select(ar => ar?.ID ?? 0);
        var userName = await currentUserName.Value();
        var user = await userContext.User(userName);
        bool hasAccess = false;
        if (user.UserName().Equals(AppUserName.Anon))
        {
            hasAccess = resourceAccess.IsAnonymousAllowed;
        }
        else if (!resourceAccess.Allowed.Any())
        {
            hasAccess = true;
        }
        else
        {
            var path = pathAccessor.Value();
            var version = await app.Version(path.Version);
            var group = await version.ResourceGroup(path.Group);
            var modCategory = await group.ModCategory();
            var modifier = await modCategory.ModifierOrDefault(path.Modifier);
            var userRoles = await user.Roles(modifier);
            var userRoleIDs = userRoles.Select(ur => ur.ID);
            var denyAccessRole = roles.First
            (
                r => r.Name().Equals(AppRoleName.DenyAccess)
            );
            if (userRoleIDs.Contains(denyAccessRole.ID))
            {
                hasAccess = false;
            }
            else if (userRoleIDs.Intersect(allowedRoleIDs).Any())
            {
                hasAccess = true;
            }
        }
        return hasAccess;
    }
}