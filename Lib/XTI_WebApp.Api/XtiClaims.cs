using Microsoft.AspNetCore.Http;

namespace XTI_WebApp.Api;

public sealed class XtiClaims
{
    private readonly HttpContext httpContext;

    public XtiClaims(IHttpContextAccessor httpContextAccessor)
        : this(httpContextAccessor.HttpContext ?? throw new ArgumentException("HttpContext is null"))
    {
    }

    public XtiClaims(HttpContext httpContext)
    {
        this.httpContext = httpContext;
    }

    public string SessionKey() => claim("SessionKey");

    public AppUserName UserName()
    {
        var userNameValue = claim("UserName");
        return string.IsNullOrWhiteSpace(userNameValue)
            ? AppUserName.Anon
            : new AppUserName(userNameValue);
    }

    private string claim(string type)
    {
        var httpUser = httpContext?.User;
        return httpUser?.Identity?.IsAuthenticated == true
            ? httpUser.Claims.First(c => c.Type == type).Value
            : "";
    }
}