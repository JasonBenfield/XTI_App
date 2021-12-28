using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiUser : IAppApiUser
{
    private readonly IAppContext appContext;
    private readonly IUserContext userContext;
    private readonly IXtiPathAccessor pathAccessor;

    public AppApiUser(IAppContext appContext, IUserContext userContext, IXtiPathAccessor pathAccessor)
    {
        this.appContext = appContext;
        this.userContext = userContext;
        this.pathAccessor = pathAccessor;
    }

    public async Task<bool> HasAccess(ResourceAccess resourceAccess)
    {
        var app = await appContext.App();
        var roles = await app.Roles();
        var allowedRoleIDs = resourceAccess.Allowed
            .Select(ar => roles.FirstOrDefault(r => r.Name().Equals(ar)))
            .Select(ar => ar?.ID.Value ?? 0);
        var user = await userContext.User();
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
            var modifier = await modCategory.Modifier(path.Modifier);
            var userRoles = await user.Roles(modifier);
            var userRoleIDs = userRoles.Select(ur => ur.ID.Value);
            var denyAccessRole = roles.First
            (
                r => r.Name().Equals(AppRoleName.DenyAccess)
            );
            if (userRoleIDs.Contains(denyAccessRole.ID.Value))
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