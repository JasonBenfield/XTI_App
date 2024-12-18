using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class ResourceAccess
{
    public static ResourceAccess AllowAnonymous() => new([], true);

    public static ResourceAccess AllowAuthenticated() => new([], false);

    public ResourceAccess(params AppRoleName[] allowed)
        : this(allowed, false)
    {
    }

    private ResourceAccess(AppRoleName[] allowed, bool isAnonAllowed)
    {
        Allowed = allowed;
        IsAnonymousAllowed = isAnonAllowed;
    }

    public AppRoleName[] Allowed { get; }
    public bool IsAnonymousAllowed { get; }
    
    public ResourceAccess WithAllowed(params AppRoleName[] allowed) => 
        new(Allowed.Union(allowed).Distinct().ToArray());

    public ResourceAccess WithoutAllowed(params AppRoleName[] denied) => 
        new(Allowed.Except(denied).Distinct().ToArray());

    public override string ToString()
    {
        var allowed = string.Join(",", Allowed.Select(r => r.DisplayText));
        return $"{nameof(ResourceAccess)}\r\nAllowed: {allowed}";
    }
}