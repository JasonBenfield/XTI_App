using XTI_App.Api;
using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed class UserGroup : AppApiGroupWrapper
{
    public UserGroup(AppApiGroup source) : base(source)
    {
        GetUserAccess = source.AddAction<ResourcePath[], ResourcePathAccess[]>()
            .Named(nameof(GetUserAccess))
            .WithExecution<GetUserAccessAction>()
            .ThrottleRequestLogging().ForOneHour()
            .ThrottleExceptionLogging().For(5).Minutes()
            .Build();
        UserProfile = source.AddAction<EmptyRequest, WebRedirectResult>()
            .Named(nameof(UserProfile))
            .WithExecution<UserProfileAction>()
            .Build();
        GetMenuLinks = source.AddAction<string, LinkModel[]>()
            .Named(nameof(GetMenuLinks))
            .WithExecution<GetMenuLinksAction>()
            .ThrottleRequestLogging().ForOneHour()
            .ThrottleExceptionLogging().For(5).Minutes()
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