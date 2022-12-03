using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net;
using XTI_App.Api;
using XTI_Core;
using XTI_TempLog;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public sealed class SessionLogMiddleware
{
    private readonly RequestDelegate _next;

    public SessionLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync
    (
        HttpContext context,
        CurrentSession currentSession,
        TempLogSession tempLogSession,
        IAnonClient anonClient,
        IClock clock,
        XtiRequestContext xtiRequestContext
    )
    {
        anonClient.Load();
        if (isAnonSessionExpired(anonClient, clock))
        {
            expireAnonSession(anonClient);
        }
        if (context.User.Identity?.IsAuthenticated == true)
        {
            currentSession.SessionKey = new XtiClaims(context).SessionKey();
        }
        else
        {
            currentSession.SessionKey = anonClient.SessionKey;
        }
        var session = await tempLogSession.StartSession();
        if (anonClient.SessionKey != session.SessionKey)
        {
            anonClient.Persist(session.SessionKey, clock.Now().AddHours(4), session.RequesterKey);
        }
        await tempLogSession.StartRequest($"{context.Request.PathBase}{context.Request.Path}");
        try
        {
            await _next(context);
            if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
            {
                var message = $"Request failed with error {context.Response.StatusCode}. Url: {context.Request.GetDisplayUrl()}";
                var caption = "An unexpected http error occurred";
                if(context.Response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    message = $"'{context.Request.GetDisplayUrl()}' was not found";
                    caption = "Not Found";
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    message = $"'{context.Request.GetDisplayUrl()}' is forbidden";
                    caption = "Forbidden";
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    message = $"'{context.Request.GetDisplayUrl()}' is unauthorized";
                    caption = "Unauthorized";
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.BadGateway)
                {
                    message = $"Bad Gateway for '{context.Request.GetDisplayUrl()}'";
                    caption = "Bad Gateway";
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.UnsupportedMediaType)
                {
                    message = $"Unsupported Media Type for '{context.Request.GetDisplayUrl()}'";
                    caption = "Unsupported Media Type";
                }
                xtiRequestContext.Failed(message, caption);
                await tempLogSession.LogError
                (
                    AppEventSeverity.Values.CriticalError,
                    message,
                    "",
                    caption
                );
            }
        }
        catch (Exception ex)
        {
            xtiRequestContext.Failed(ex);
            await logException(tempLogSession, ex);
        }
        finally
        {
            await tempLogSession.EndRequest();
        }
    }

    private static bool isAnonSessionExpired(IAnonClient anonClient, IClock clock)
    {
        return !string.IsNullOrWhiteSpace(anonClient.SessionKey) && clock.Now().ToUniversalTime() > anonClient.SessionExpirationTime.ToUniversalTime();
    }

    private static void expireAnonSession(IAnonClient anonClient)
    {
        anonClient.Persist("", DateTimeOffset.MinValue, anonClient.RequesterKey);
    }

    private static async Task logException(TempLogSession sessionLog, Exception ex)
    {
        AppEventSeverity severity;
        string caption;
        if (ex is ValidationFailedException)
        {
            severity = AppEventSeverity.Values.ValidationFailed;
            caption = "Validation Failed";
        }
        else if (ex is AccessDeniedException accessDeniedException)
        {
            severity = AppEventSeverity.Values.AccessDenied;
            caption = accessDeniedException.DisplayMessage;
        }
        else if (ex is AppException appException)
        {
            severity = AppEventSeverity.Values.AppError;
            caption = appException.DisplayMessage;
        }
        else
        {
            severity = AppEventSeverity.Values.CriticalError;
            caption = "An unexpected error occurred";
        }
        await sessionLog.LogException(severity, ex, caption);
    }

}