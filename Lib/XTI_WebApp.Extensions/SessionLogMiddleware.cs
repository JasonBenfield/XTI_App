using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
        Console.WriteLine("SessionLogMiddleware Start");
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
            Console.WriteLine("SessionLogMiddleware Before next");
            await _next(context);
            Console.WriteLine("SessionLogMiddleware After next");
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
        Console.WriteLine("SessionLogMiddleware End");
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