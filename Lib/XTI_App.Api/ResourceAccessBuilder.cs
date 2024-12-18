using System.Reflection.Metadata.Ecma335;
using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class ResourceAccessBuilder
{
    private ResourceAccessBuilder? defaultAccess;
    private bool isAnonymousAllowed;
    private readonly List<AppRoleName> allowedRoles = new();
    private readonly List<AppRoleName> deniedRoles = new();

    public ResourceAccessBuilder()
    {
    }

    public ResourceAccessBuilder(ResourceAccessBuilder defaultAccess)
    {
        this.defaultAccess = defaultAccess;
    }

    public ResourceAccessBuilder AllowAnonymous()
    {
        defaultAccess = null;
        isAnonymousAllowed = true;
        allowedRoles.Clear();
        deniedRoles.Clear();
        return this;
    }

    public ResourceAccessBuilder Reset(ResourceAccess access)
    {
        defaultAccess = null;
        isAnonymousAllowed = access.IsAnonymousAllowed;
        allowedRoles.AddRange(access.Allowed);
        deniedRoles.Clear();
        return this;
    }

    public ResourceAccessBuilder Reset() => Reset([]);

    public ResourceAccessBuilder Reset(params AppRoleName[] allowedRoleNames)
    {
        defaultAccess = null;
        allowedRoles.AddRange(allowedRoleNames);
        deniedRoles.Clear();
        return this;
    }

    public ResourceAccessBuilder WithAllowed(params AppRoleName[] roleNames)
    {
        allowedRoles.AddRange(roleNames);
        return this;
    }

    public ResourceAccessBuilder WithoutAllowed(params AppRoleName[] roleNames)
    {
        deniedRoles.AddRange(roleNames);
        return this;
    }

    public ResourceAccess Build()
    {
        ResourceAccess access;
        if (isAnonymousAllowed)
        {
            access = ResourceAccess.AllowAnonymous();
        }
        else
        {
            access = defaultAccess?.Build() ?? ResourceAccess.AllowAuthenticated();
            if (allowedRoles.Any())
            {
                access = access.WithAllowed(allowedRoles.Distinct().ToArray());
            }
            if (deniedRoles.Any())
            {
                access = access.WithoutAllowed(deniedRoles.Distinct().ToArray());
            }
        }
        return access;
    }
}
