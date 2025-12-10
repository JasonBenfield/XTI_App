using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public sealed class CurrentVersionMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentVersionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, XtiEnvironment xtiEnv, CacheBust cacheBust)
    {
        var request = context.Request;
        var path = $"{request?.Path}";
        if (request != null && request.Method == "GET" && path.IndexOf(".") == -1)
        {
            if (xtiEnv.IsProduction())
            {
                var oldQueryCacheBust =
                    request.Query["cacheBust"].FirstOrDefault() ??
                    "";
                var queryCacheBust =
                    request.Query["v"].FirstOrDefault() ??
                    "";
                var versionKey = context.RequestServices.GetRequiredService<AppVersionKey>();
                if (versionKey.IsCurrent())
                {
                    var url = request.GetDisplayUrl();
                    var cacheBustValue = await cacheBust.Value();
                    if (!string.IsNullOrWhiteSpace(oldQueryCacheBust))
                    {
                        url = url
                            .Replace
                            (
                                $"cacheBust={oldQueryCacheBust}",
                                ""
                            );
                    }
                    if (string.IsNullOrWhiteSpace(queryCacheBust))
                    {
                        var delimiter = url.Contains("?") ? "&" : "?";
                        url = $"{url}{delimiter}v={cacheBustValue}";
                        context.Response.Redirect(url);
                        return;
                    }
                    else if (queryCacheBust != cacheBustValue)
                    {
                        url = url
                            .Replace
                            (
                                $"v={queryCacheBust}",
                                $"v={cacheBustValue}"
                            );
                        context.Response.Redirect(url);
                        return;
                    }
                    if (!string.IsNullOrWhiteSpace(oldQueryCacheBust))
                    {
                        context.Response.Redirect(url);
                        return;
                    }
                }
            }
        }
        await _next(context);
    }
}
