using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;

namespace XTI_WebApp.Extensions;

public sealed class WebXtiPathAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly XtiBasePath xtiBasePath;

    public WebXtiPathAccessor(IHttpContextAccessor httpContextAccessor, XtiBasePath xtiBasePath)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.xtiBasePath = xtiBasePath;
    }

    public XtiPath Value()
    {
        var request = httpContextAccessor.HttpContext?.Request;
        var pathText = $"{request?.PathBase}{request?.Path}";
        XtiPath xtiPath;
        if (string.IsNullOrWhiteSpace(pathText))
        {
            xtiPath = xtiBasePath.Value;
        }
        else
        {
            var parsedXtiPath = XtiPath.Parse(pathText);
            xtiPath = xtiBasePath
                .Value
                .WithGroup(parsedXtiPath.Group)
                .WithAction(parsedXtiPath.Action)
                .WithModifier(parsedXtiPath.Modifier);
        }
        return xtiPath;
    }
}