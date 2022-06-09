using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using XTI_Core;

namespace XTI_WebApp.Extensions;

public sealed class TerminateMiddleware
{
    private readonly RequestDelegate _next;

    public TerminateMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, XtiEnvironment xtiEnv, IHostApplicationLifetime lifetime)
    {
        if (xtiEnv.IsTest() && context.Request.Path.Value?.Equals("/StopApp") == true)
        {
            lifetime.StopApplication();
        }
        else
        {
            await _next(context);
        }
    }
}