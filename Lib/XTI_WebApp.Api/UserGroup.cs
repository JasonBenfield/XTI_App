using Microsoft.Extensions.DependencyInjection;
using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class UserGroup : AppApiGroupWrapper
{
    public UserGroup(AppApiGroup source, IServiceProvider sp) : base(source)
    {
        Index = source.AddAction
        (
            nameof(Index),
            () => ViewAppAction<UserStartRequest>.Index()
        );
        AccessDenied = source.AddAction
        (
            nameof(AccessDenied),
            () => new AccessDeniedAction()
        );
        Error = source.AddAction
        (
            nameof(Error),
            () => new ErrorAction()
        );
        Logout = source.AddAction
        (
            nameof(Logout), 
            () => sp.GetRequiredService<LogoutAction>()
        );
    }

    public AppApiAction<UserStartRequest, WebViewResult> Index { get; }

    public AppApiAction<EmptyRequest, WebViewResult> AccessDenied { get; }

    public AppApiAction<EmptyRequest, WebViewResult> Error { get; }

    public AppApiAction<LogoutRequest, WebRedirectResult> Logout { get; }
}