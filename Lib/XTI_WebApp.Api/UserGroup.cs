using Microsoft.Extensions.DependencyInjection;
using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class UserGroup : AppApiGroupWrapper
{
    public UserGroup(AppApiGroup source, IServiceProvider sp) : base(source)
    {
        var actions = new WebAppApiActionFactory(source);
        Index = source.AddAction(actions.DefaultView<UserStartRequest>());
        AccessDenied = source.AddAction
        (
            actions.View(nameof(AccessDenied), () => new AccessDeniedAction())
        );
        Error = source.AddAction
        (
            actions.View(nameof(Error), () => new ErrorAction())
        );
        Logout = source.AddAction
        (
            actions.Action(nameof(Logout), () => sp.GetRequiredService<LogoutAction>())
        );
    }

    public AppApiAction<UserStartRequest, WebViewResult> Index { get; }

    public AppApiAction<EmptyRequest, WebViewResult> AccessDenied { get; }

    public AppApiAction<EmptyRequest, WebViewResult> Error { get; }

    public AppApiAction<LogoutRequest, WebRedirectResult> Logout { get; }
}