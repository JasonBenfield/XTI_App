using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed class UserGroup : AppApiGroupWrapper
{
    public UserGroup(AppApiGroup source, IServiceProvider sp) : base(source)
    {
        GetUserAccess = source.AddAction<ResourcePath[], ResourcePathAccess[]>()
            .Named(nameof(GetUserAccess))
            .WithExecution<GetUserAccessAction>()
            .Build();
        UserProfile = source.AddAction<EmptyRequest, WebRedirectResult>()
            .Named(nameof(UserProfile))
            .WithExecution<UserProfileAction>()
            .Build();
        GetMenuLinks = source.AddAction<string, LinkModel[]>()
            .Named(nameof(GetMenuLinks))
            .WithExecution<GetMenuLinksAction>()
            .Build();
        Logout = source.AddAction<LogoutRequest, WebRedirectResult>()
            .Named(nameof(Logout))
            .WithExecution<LogoutAction>()
            .Build();
    }

    public AppApiAction<ResourcePath[], ResourcePathAccess[]> GetUserAccess { get; }

    public AppApiAction<EmptyRequest, WebRedirectResult> UserProfile { get; }

    public AppApiAction<string, LinkModel[]> GetMenuLinks { get; }

    public AppApiAction<LogoutRequest, WebRedirectResult> Logout { get; }
}