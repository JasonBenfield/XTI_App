using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;

namespace XTI_WebApp.Extensions;

public sealed class WebXtiPathAccessor : IXtiPathAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public WebXtiPathAccessor(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public XtiPath Value()
    {
        var request = httpContextAccessor.HttpContext?.Request;
        return XtiPath.Parse($"{request?.PathBase}{request?.Path}");
    }
}