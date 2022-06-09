using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using XTI_TempLog;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public sealed class LogoutProcess : ILogoutProcess
{
    private readonly TempLogSession tempLogSession;
    private readonly IHttpContextAccessor httpContextAccessor;

    public LogoutProcess(TempLogSession tempLogSession, IHttpContextAccessor httpContextAccessor)
    {
        this.tempLogSession = tempLogSession;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task Run()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            try
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch { }
        }
        await tempLogSession.EndSession();
    }
}
