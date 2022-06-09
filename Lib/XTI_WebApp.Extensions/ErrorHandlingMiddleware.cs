using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using XTI_App.Api;
using XTI_Core;

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
        ICompositeViewEngine viewEngine,
        XtiEnvironment xtiEnv
    )
    {
        await _next(context);
        if (xtiRequestContext.HasError())
        {
            var ex = xtiRequestContext.Error();
            context.Response.StatusCode = getErrorStatusCode(ex);
            if (IsApiRequest(context.Request))
            {
                context.Response.ContentType = "application/json";
                var errors = new ResultContainer<ErrorModel[]>(getErrors(xtiEnv, ex));
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
            && request.ContentType?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) == true;

    private int getErrorStatusCode(Exception ex)
    {
        int statusCode;
        if (ex is AppException)
        {
            if (ex is AccessDeniedException)
            {
                statusCode = StatusCodes.Status403Forbidden;
            }
            else
            {
                statusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
        }
        return statusCode;
    }

    private ErrorModel[] getErrors(XtiEnvironment xtiEnv, Exception ex)
    {
        ErrorModel[] errors;
        if (xtiEnv.IsDevelopmentOrTest())
        {
            if (ex is ValidationFailedException validationFailedException)
            {
                errors = validationFailedException.Errors.ToArray();
            }
            else
            {
                errors = new[] { new ErrorModel(ex.StackTrace ?? "", ex.Message, "") };
            }
        }
        else if (ex is AppException appException)
        {
            if (ex is ValidationFailedException validationFailedException)
            {
                errors = validationFailedException.Errors.ToArray();
            }
            else
            {
                errors = new[] { new ErrorModel(appException.DisplayMessage) };
            }
        }
        else
        {
            errors = new[] { new ErrorModel("An unexpected error occurred") };
        }
        return errors;
    }

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
        // get the view and attach the model to view data
        var view = viewEngineResult?.View;
        if (view == null)
        {
            throw new FileNotFoundException("View cannot be found.");
        }
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