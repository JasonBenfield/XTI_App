using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using XTI_App.Abstractions;
using XTI_App.Api;
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
        XtiRequestContext xtiRequestContext,
        IAppApi api
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
        var xtiPath = XtiPath.Parse(path);
        var requestDataLoggingType = RequestDataLoggingTypes.Never;
        var isResultDataLoggingEnabled = false;
        if (api.HasAction(xtiPath))
        {
            var action = api.GetAction(xtiPath);
            requestDataLoggingType = action.RequestDataLoggingType;
            isResultDataLoggingEnabled = action.IsResultDataLoggingEnabled;
        }
        try
        {
            var originalResponseBody = context.Response.Body;
            MemoryStream? newResponseBody = null;
            if (isResultDataLoggingEnabled)
            {
                newResponseBody = new MemoryStream();
                context.Response.Body = newResponseBody;
            }
            await _next(context);
            if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 400)
            {
                await LogHttpError(context, tempLogSession, xtiRequestContext);
            }
            else
            {
                if (requestDataLoggingType == RequestDataLoggingTypes.Always)
                {
                    await LogRequestData(context, tempLogSession);
                }
                if (isResultDataLoggingEnabled)
                {
                    newResponseBody!.Seek(0, SeekOrigin.Begin);
                    using var streamReader = new StreamReader(newResponseBody);
                    var responseBody = await streamReader.ReadToEndAsync();
                    var match = dataContainerRegex.Match(responseBody);
                    if (match.Success)
                    {
                        responseBody = match.Groups["ResultData"].Value;
                    }
                    await tempLogSession.LogResultData(responseBody);
                    newResponseBody.Seek(0, SeekOrigin.Begin);
                    await newResponseBody.CopyToAsync(originalResponseBody);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in {path}\r\n{ex}");
            if(requestDataLoggingType == RequestDataLoggingTypes.OnError)
            {
                await LogRequestData(context, tempLogSession);
            }
            var loggedException = await LogException(tempLogSession, ex);
            xtiRequestContext.Failed(ex, loggedException.EventKey);
        }
        finally
        {
            await tempLogSession.EndRequest();
        }
    }

    private static async Task LogHttpError(HttpContext context, TempLogSession tempLogSession, XtiRequestContext xtiRequestContext)
    {
        var path = $"{context.Request.PathBase}{context.Request.Path}";
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

    private static async Task LogRequestData(HttpContext context, TempLogSession tempLogSession)
    {
        try
        {
            var requestData = await new BodyFromRequest(context.Request).Serialize();
            await tempLogSession.LogRequestData(requestData);
        }
        catch { }
    }

    private static readonly Regex dataContainerRegex = new Regex("^{\"Data\":(?<ResultData>.*)}$");

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