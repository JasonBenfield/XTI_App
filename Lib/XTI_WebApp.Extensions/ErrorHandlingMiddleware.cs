using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;
using XTI_App.Abstractions;

namespace XTI_WebApp.Extensions;

internal sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync
    (
        HttpContext context,
        XtiRequestContext xtiRequestContext,
        ICompositeViewEngine viewEngine
    )
    {
        await _next(context);
        if (xtiRequestContext.HasError())
        {
            try
            {
                var statusCode = xtiRequestContext.GetStatusCode();
                if (statusCode.HasValue)
                {
                    context.Response.StatusCode = statusCode.Value;
                }
            }
            catch { }
            if (IsApiRequest(context.Request))
            {
                context.Response.ContentType = "application/json";
                var errors = ResultContainer.Create(xtiRequestContext.GetErrorResult());
                var serializedErrors = JsonSerializer.Serialize(errors);
                await context.Response.WriteAsync(serializedErrors);
            }
            else
            {
                string viewPath;
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    viewPath = "User/AccessDenied";
                }
                else
                {
                    viewPath = "User/Error";
                }
                var view = await RenderViewToString(viewEngine, context, viewPath);
                await context.Response.WriteAsync(view);
            }
        }
    }

    private static bool IsApiRequest(HttpRequest request)
        => request != null
            && request.Method == "POST"
            &&
            (
                request.ContentType?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) == true ||
                request.ContentType?.StartsWith("text/plain", StringComparison.OrdinalIgnoreCase) == true
            );

    private static async Task<string> RenderViewToString(IViewEngine viewEngine, HttpContext httpContext, string viewPath, object? model = null)
    {
        var actionDescriptor = new ActionDescriptor
        {
            RouteValues = new Dictionary<string, string?>()
        };
        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            ActionDescriptor = actionDescriptor,
            RouteData = new RouteData()
        };
        var viewEngineResult = viewEngine.FindView(actionContext, viewPath, false);
        var view = viewEngineResult?.View ?? throw new FileNotFoundException("View cannot be found.");
        string result;
        using (var sw = new StringWriter())
        {
            var ctx = new ViewContext()
            {
                ActionDescriptor = actionDescriptor,
                HttpContext = httpContext,
                RouteData = new RouteData(),
                Writer = sw,
                View = view
            };
            await view.RenderAsync(ctx);
            sw.Flush();
            result = sw.ToString();
        }
        return result;
    }
}