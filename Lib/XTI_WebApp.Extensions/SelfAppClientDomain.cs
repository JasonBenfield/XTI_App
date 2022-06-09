using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;
using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Extensions;

public sealed class SelfAppClientDomain : IAppClientDomain
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly AppKey appKey;

    public SelfAppClientDomain(IHttpContextAccessor httpContextAccessor, AppKey appKey)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.appKey = appKey;
    }

    public Task<string> Value(string appName, string version)
    {
        string domain;
        if (appKey.Name.Equals(appName))
        {
            domain = httpContextAccessor.HttpContext?.Request.Host.Value ?? "";
        }
        else
        {
            domain = "";
        }
        return Task.FromResult(domain);
    }
}