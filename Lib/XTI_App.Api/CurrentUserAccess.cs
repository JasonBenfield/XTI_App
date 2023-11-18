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

    public async Task<bool> IsAnon()
    {
        var userName = await currentUserName.Value();
        return userName.IsAnon();
    }

    public Task<UserAccessResult> HasAccess(XtiPath xtiPath) =>
        HasAccess(xtiPath.Group, xtiPath.Action, xtiPath.Modifier);

    public async Task<UserAccessResult> HasAccess(ResourceGroupName group, ResourceName action, ModifierKey modKey)
    {
        var error = "";
        var userName = await currentUserName.Value();
        var userContextModel = await userContext.User(userName);
        if (!userContextModel.User.IsActive())
        {
            error = "User has been deactivated";
        }
        else
        {
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
            if (userName.IsAnon())
            {
                if (!isAnonymousAllowed)
                {
                    error = $"Anon is not allowed to '{FormatPath(group, action, modKey)}'";
                }
            }
            else if (allowedRoles.Any())
            {
                var modifier = appContextModel.Modifier(group, modKey);
                var userRoles = userContextModel.GetRoles(modifier);
                var userRoleNames = userRoles.Select(ur => ur.Name);
                if (userRoles.Any(ur => ur.IsDenyAccess()))
                {
                    error = $"'{userName.DisplayText}' is denied access to '{FormatPath(group, action, modKey)}'";
                }
                else if (!userRoleNames.Intersect(allowedRoles).Any())
                {
                    var joinedAllowedRoles = string.Join(",", allowedRoles.Select(ar => ar.DisplayText));
                    var joinedUserRoles = string.Join(",", userRoleNames.Select(ar => ar.DisplayText));
                    error = $"'{userName.DisplayText}' is not allowed to '{FormatPath(group, action, modKey)}'. Allowed roles: {joinedAllowedRoles}. User roles: {joinedUserRoles}";
                }
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

    private string FormatPath(ResourceGroupName group, ResourceName action, ModifierKey modifier)
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
