using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
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
                var versionKey = context.RequestServices.GetRequiredService<AppVersionKey>();
                if (versionKey.IsCurrent())
                {
                    var cacheBustValue = await cacheBust.Value(context.RequestAborted);
                    var parsedQueryString = QueryHelpers.ParseQuery(request.QueryString.Value ?? "");
                    if (parsedQueryString.ContainsKey("cacheBust"))
                    {
                        parsedQueryString.Remove("cacheBust");
                    }
                    if (parsedQueryString.ContainsKey("v"))
                    {
                        var v = parsedQueryString["v"].FirstOrDefault() ?? "";
                        if (v != cacheBustValue)
                        {
                            parsedQueryString["v"] = new StringValues(cacheBustValue);
                        }
                    }
                    else
                    {
                        parsedQueryString.Add("v", cacheBustValue);
                    }
                    var originalUrl = request.GetDisplayUrl();
                    var uri = new Uri(originalUrl);
                    var newUrl = QueryHelpers.AddQueryString(uri.GetLeftPart(UriPartial.Path), parsedQueryString);
                    if(newUrl != originalUrl)
                    {
                        context.Response.Redirect(newUrl);
                        return;
                    }
                }
            }
        }
        await _next(context);
    }
}
