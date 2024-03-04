using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
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
                var queryCacheBust = request.Query["cacheBust"].FirstOrDefault() ?? "";
                var versionKey = context.RequestServices.GetRequiredService<AppVersionKey>();
                if (versionKey.IsCurrent())
                {
                    var url = request.GetDisplayUrl();
                    var cacheBustValue = await cacheBust.Value();
                    if (string.IsNullOrWhiteSpace(queryCacheBust))
                    {
                        var delimiter = url.Contains("?") ? "&" : "?";
                        url = $"{url}{delimiter}cacheBust={cacheBustValue}";
                        context.Response.Redirect(url);
                        return;
                    }
                    else if (queryCacheBust != cacheBustValue)
                    {
                        url = url.Replace
                            (
                                $"cacheBust={queryCacheBust}",
                                $"cacheBust={cacheBustValue}"
                            );
                        context.Response.Redirect(url);
                        return;
                    }
                }
            }
        }
        await _next(context);
    }
}
