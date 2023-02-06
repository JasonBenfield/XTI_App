using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed class UserGroup : AppApiGroupWrapper
{
    public UserGroup(AppApiGroup source, IServiceProvider sp) : base(source)
    {
        GetUserAccess = source.AddAction
        (
            nameof(GetUserAccess),
            () => sp.GetRequiredService<GetUserAccessAction>()
        );
        UserProfile = source.AddAction
        (
            nameof(UserProfile),
            () => sp.GetRequiredService<UserProfileAction>()
        );
        GetMenuLinks = source.AddAction
        (
            nameof(GetMenuLinks),
            () => sp.GetRequiredService<GetMenuLinksAction>()
        );
        Logout = source.AddAction
        (
            nameof(Logout),
            () => sp.GetRequiredService<LogoutAction>()
        );
    }

    public AppApiAction<ResourcePath[], ResourcePathAccess[]> GetUserAccess { get; }

    public AppApiAction<EmptyRequest, WebRedirectResult> UserProfile { get; }

    public AppApiAction<string, LinkModel[]> GetMenuLinks { get; }

    public AppApiAction<LogoutRequest, WebRedirectResult> Logout { get; }
}