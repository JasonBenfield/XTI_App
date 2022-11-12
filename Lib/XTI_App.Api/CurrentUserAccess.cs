using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class CurrentUserAccess
{
    private readonly IUserContext userContext;
    private readonly IAppContext appContext;
    private readonly ICurrentUserName currentUserName;

    public CurrentUserAccess(IUserContext userContext, IAppContext appContext, ICurrentUserName currentUserName)
    {
        this.userContext = userContext;
        this.appContext = appContext;
        this.currentUserName = currentUserName;
    }

    public async Task<UserAccessResult> HasAccess(ResourceGroupName group, ResourceName action, ModifierKey modifier)
    {
        var error = "";
        var userName = await currentUserName.Value();
        var userContextModel = await userContext.User(userName);
        var appContextModel = await appContext.App();
        bool isAnonymousAllowed;
        AppRoleName[] allowedRoles;
        if (action.IsBlank())
        {
            var resourceGroup = appContextModel.ResourceGroup(group);
            isAnonymousAllowed = resourceGroup.ResourceGroup.IsAnonymousAllowed;
            allowedRoles = resourceGroup.AllowedRoles.Select(ar => ar.Name).ToArray();
        }
        else
        {
            var resource = appContextModel.Resource(group, action);
            isAnonymousAllowed = resource.Resource.IsAnonymousAllowed;
            allowedRoles = resource.AllowedRoles.Select(ar => ar.Name).ToArray();
        }
        if (userName.Equals(AppUserName.Anon))
        {
            if (!isAnonymousAllowed)
            {
                error = $"Anon is not allowed to '{formatPath(group, action, modifier)}'";
            }
        }
        else if (allowedRoles.Any())
        {
            var modCategory = appContextModel.ModCategory(group);
            var userRoles = userContextModel.GetRoles(modCategory.ModifierCategory.ID, modifier);
            var userRoleNames = userRoles.Select(ur => ur.Name);
            if (userRoles.Any(ur => ur.Name.Equals(AppRoleName.DenyAccess)))
            {
                error = $"'{userName.DisplayText}' is denied access to '{formatPath(group, action, modifier)}'";
            }
            else if (!userRoleNames.Intersect(allowedRoles).Any())
            {
                var joinedAllowedRoles = string.Join(",", allowedRoles.Select(ar => ar.DisplayText));
                var joinedUserRoles = string.Join(",", userRoleNames.Select(ar => ar.DisplayText));
                error = $"'{userName.DisplayText}' is not allowed to '{formatPath(group, action, modifier)}'. Allowed roles: {joinedAllowedRoles}. User roles: {joinedUserRoles}";
            }
        }
        UserAccessResult result;
        if (string.IsNullOrWhiteSpace(error))
        {
            result = UserAccessResult.Authorized();
        }
        else
        {
            result = UserAccessResult.Denied(error);
        }
        return result;
    }

    private string formatPath(ResourceGroupName group, ResourceName action, ModifierKey modifier)
    {
        var path = group.DisplayText;
        if (!action.IsBlank())
        {
            path += $"/{action.DisplayText}";
            if (!modifier.IsBlank())
            {
                path += $"/{modifier.DisplayText}";
            }
        }
        return path;
    }
}
