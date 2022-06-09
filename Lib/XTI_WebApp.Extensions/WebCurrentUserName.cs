using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public sealed class WebCurrentUserName : ICurrentUserName
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public WebCurrentUserName(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Task<AppUserName> Value()
    {
        var userName = new XtiClaims(httpContextAccessor).UserName();
        return Task.FromResult(userName);
    }
}
