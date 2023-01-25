using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_TempLog;
using XTI_TempLog.Abstractions;
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
                var loggedError = await tempLogSession.LogError
                (
                    AppEventSeverity.Values.CriticalError,
                    message,
                    "",
                    caption,
                    ""
                );
                xtiRequestContext.Failed(message, caption, loggedError.EventKey);
            }
        }
        catch (Exception ex)
        {
            var loggedException = await logException(tempLogSession, ex);
            xtiRequestContext.Failed(ex, loggedException.EventKey);
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

    private static Task<LogEntryModel> logException(TempLogSession sessionLog, Exception ex)
    {
        var severity = new SeverityFromException(ex).Value;
        string caption;
        if (ex is ValidationFailedException)
        {
            caption = "Validation Failed";
        }
        else if (ex is AppException appException)
        {
            caption = appException.DisplayMessage;
        }
        else
        {
            caption = "An unexpected error occurred";
        }
        var parentEventKey = "";
        if(ex is AppClientException clientEx)
        {
            parentEventKey = clientEx.LogEntryKey;
        }
        return sessionLog.LogException(severity, ex, caption, parentEventKey);
    }

}