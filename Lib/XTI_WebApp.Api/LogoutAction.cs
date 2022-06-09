using System.Web;
using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class LogoutAction : AppAction<LogoutRequest, WebRedirectResult>
{
    private readonly ILogoutProcess logoff;
    private readonly LoginUrl loginUrl;

    public LogoutAction(ILogoutProcess logoff, LoginUrl loginUrl)
    {
        this.logoff = logoff;
        this.loginUrl = loginUrl;
    }

    public async Task<WebRedirectResult> Execute(LogoutRequest model, CancellationToken stoppingToken)
    {
        await logoff.Run();
        var authUrl = await loginUrl.Value(HttpUtility.UrlDecode(model.ReturnUrl));
        return new WebRedirectResult(authUrl);
    }
}
