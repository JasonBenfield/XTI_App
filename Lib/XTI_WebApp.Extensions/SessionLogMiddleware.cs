using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;
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
        IAppEnvironmentContext appEnvironmentContext,
        IAnonClient anonClient,
        IClock clock,
        XtiRequestContext xtiRequestContext
    )
    {
        anonClient.Load();
        if (IsAnonSessionExpired(anonClient, clock))
        {
            ExpireAnonSession(anonClient);
        }
        var sourceSessionKey = SessionKey.Parse(context.Request.Headers[new SourceSessionKeyHeader().Value].FirstOrDefault() ?? "");
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var xtiClaims = new XtiClaims(context);
            var userName = xtiClaims.UserName().Value;
            if (sourceSessionKey.IsEmpty() || !sourceSessionKey.HasUserName(userName))
            {
                currentSession.SessionKey = xtiClaims.SessionKey();
            }
            else
            {
                currentSession.SessionKey = sourceSessionKey;
            }
        }
        else if (!sourceSessionKey.IsEmpty())
        {
            currentSession.SessionKey = sourceSessionKey;
        }
        else
        {
            currentSession.SessionKey = SessionKey.Parse(anonClient.SessionKey);
        }
        if (currentSession.SessionKey.IsEmpty())
        {
            var session = await tempLogSession.StartSession();
            currentSession.SessionKey = session.SessionKey;
        }
        if (!currentSession.SessionKey.Equals(anonClient.SessionKey) || string.IsNullOrWhiteSpace(anonClient.RequesterKey))
        {
            var appEnv = await appEnvironmentContext.Value();
            anonClient.Persist(currentSession.SessionKey.Format(), clock.Now().AddHours(4), appEnv.RequesterKey);
        }
        var path = $"{context.Request.PathBase}{context.Request.Path}";
        var sourceRequestKey = context.Request.Headers[new SourceRequestKeyHeader().Value].FirstOrDefault() ?? "";
        await tempLogSession.StartRequest(path, sourceRequestKey);
        try
        {
            await _next(context);
            if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 400)
            {
                var message = $"Request failed with error {context.Response.StatusCode}. Url: {context.Request.GetDisplayUrl()}";
                var caption = "An unexpected http error occurred";
                var statusCode = (HttpStatusCode)context.Response.StatusCode;
                if (statusCode == HttpStatusCode.NotFound)
                {
                    message = $"'{context.Request.GetDisplayUrl()}' was not found";
                    caption = "Not Found";
                }
                else if (statusCode == HttpStatusCode.Forbidden)
                {
                    message = $"'{context.Request.GetDisplayUrl()}' is forbidden";
                    caption = "Forbidden";
                }
                else if (statusCode == HttpStatusCode.Unauthorized)
                {
                    message = $"'{context.Request.GetDisplayUrl()}' is unauthorized";
                    caption = "Unauthorized";
                }
                else if (statusCode == HttpStatusCode.BadGateway)
                {
                    message = $"Bad Gateway for '{context.Request.GetDisplayUrl()}'";
                    caption = "Bad Gateway";
                }
                else if (statusCode == HttpStatusCode.UnsupportedMediaType)
                {
                    message = $"Unsupported Media Type for '{context.Request.GetDisplayUrl()}'";
                    caption = "Unsupported Media Type";
                }
                Debug.WriteLine($"Error in {path} [{context.Response.StatusCode}]\r\n{caption}\r\n{message}");
                var loggedError = await tempLogSession.LogError
                (
                    AppEventSeverity.Values.CriticalError,
                    message,
                    "",
                    caption,
                    "",
                    $"HttpError{statusCode}"
                );
                xtiRequestContext.Failed(message, caption, loggedError.EventKey);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in {path}\r\n{ex}");
            var loggedException = await LogException(tempLogSession, ex);
            xtiRequestContext.Failed(ex, loggedException.EventKey);
        }
        finally
        {
            await tempLogSession.EndRequest();
        }
    }

    private static bool IsAnonSessionExpired(IAnonClient anonClient, IClock clock) =>
        !string.IsNullOrWhiteSpace(anonClient.SessionKey) &&
        clock.Now() > anonClient.SessionExpirationTime;

    private static void ExpireAnonSession(IAnonClient anonClient) =>
        anonClient.Persist("", DateTimeOffset.MinValue, anonClient.RequesterKey);

    private static Task<LogEntryModel> LogException(TempLogSession sessionLog, Exception ex)
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
        if (ex is AppClientException clientEx)
        {
            parentEventKey = clientEx.LogEntryKey;
        }
        return sessionLog.LogException(severity, ex, caption, parentEventKey);
    }

}