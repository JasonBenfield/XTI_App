using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiUser : IAppApiUser
{
    private readonly IUserContext userContext;
    private readonly IAppContext appContext;
    private readonly ICurrentUserName currentUserName;
    private readonly IXtiPathAccessor pathAccessor;

    public AppApiUser(IUserContext userContext, IAppContext appContext, ICurrentUserName currentUserName, IXtiPathAccessor pathAccessor)
    {
        this.userContext = userContext;
        this.appContext = appContext;
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
        var userContextModel = await userContext.User(userName);
        if (userContextModel.User.UserName.Equals(AppUserName.Anon))
        {
            if (!resourceAccess.IsAnonymousAllowed)
            {
                error = $"Anon is not allowed to '{path.Format()}'";
            }
        }
        else if (resourceAccess.Allowed.Any())
        {
            var appContextModel = await appContext.App();
            var modCategory = appContextModel.ModCategory(path.Group);
            var userRoles = userContextModel.GetRoles(modCategory.ModifierCategory.ID, path.Modifier);
            var userRoleNames = userRoles.Select(ur => ur.Name);
            if (userRoles.Any(ur => ur.Name.Equals(AppRoleName.DenyAccess)))
            {
                error = $"'{userName.DisplayText}' is denied access to '{path.Format()}'";
            }
            else if (!userRoleNames.Intersect(resourceAccess.Allowed).Any())
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