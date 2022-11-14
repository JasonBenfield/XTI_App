using Microsoft.AspNetCore.Http;
using System.Web;
using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class LogoutAction : AppAction<LogoutRequest, WebRedirectResult>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogoutProcess logoff;
    private readonly LoginUrl loginUrl;

    public LogoutAction(IHttpContextAccessor httpContextAccessor, ILogoutProcess logoff, LoginUrl loginUrl)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.logoff = logoff;
        this.loginUrl = loginUrl;
    }

    public async Task<WebRedirectResult> Execute(LogoutRequest model, CancellationToken stoppingToken)
    {
        await logoff.Run();
        string returnUrl;
        try
        {
            returnUrl = HttpUtility.UrlDecode(model.ReturnUrl);
            var requestHost = httpContextAccessor.HttpContext.Request.Host.Host;
            var returnUrlHost = new Uri(returnUrl).Host;
            if(!requestHost.Equals(returnUrlHost, StringComparison.OrdinalIgnoreCase))
            {
                returnUrl = GetDefaultReturnUrl();
            }
        }
        catch
        {
            returnUrl = GetDefaultReturnUrl();
        }
        var authUrl = await loginUrl.Value(returnUrl);
        return new WebRedirectResult(authUrl);
    }

    private string GetDefaultReturnUrl()
    {
        var request = httpContextAccessor.HttpContext.Request;
        return $"{request.Scheme}://{request.Host}{request.PathBase}";
    }
}
