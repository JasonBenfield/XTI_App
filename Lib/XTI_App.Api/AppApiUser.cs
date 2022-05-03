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
        var userName = await currentUserName.Value();
        var path = pathAccessor.Value();
        var error = await HasAccess(resourceAccess, userName, path);
        return string.IsNullOrWhiteSpace(error);
    }

    private async Task<string> HasAccess(ResourceAccess resourceAccess, AppUserName userName, XtiPath path)
    {
        var error = "";
        var app = await appContext.App();
        var roles = await app.Roles();
        var allowedRoleIDs = resourceAccess.Allowed
            .Select(ar => roles.FirstOrDefault(r => r.Name().Equals(ar)))
            .Select(ar => ar?.ID ?? 0);
        var user = await userContext.User(userName);
        if (user.UserName().Equals(AppUserName.Anon))
        {
            if (!resourceAccess.IsAnonymousAllowed)
            {
                error = $"Anon is not allowed to '{path.Format()}'";
            }
        }
        else if (resourceAccess.Allowed.Any())
        {
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
                error = $"'{userName.DisplayText}' is denied access to '{path.Format()}'";
            }
            else if (!userRoleIDs.Intersect(allowedRoleIDs).Any())
            {
                var joinedRoles = string.Join(",", resourceAccess.Allowed.Select(ar => ar.DisplayText));
                error = $"'{userName.DisplayText}' is not allowed to '{path.Format()}'. Allowed roles: {joinedRoles}.";
            }
        }
        return error;
    }

    public async Task EnsureUserHasAccess(ResourceAccess resourceAccess)
    {
        var userName = await currentUserName.Value();
        var path = pathAccessor.Value();
        var error = await HasAccess(resourceAccess, userName, path);
        if (!string.IsNullOrWhiteSpace(error))
        {
            throw new AccessDeniedException(error);
        }
    }

}