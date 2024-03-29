﻿using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public sealed class XtiRequestContext
{
    private readonly XtiEnvironment xtiEnv;
    private Exception? error;
    private string message = "";
    private string caption = "";
    private string logEntryKey = "";

    public XtiRequestContext(XtiEnvironment xtiEnv)
    {
        this.xtiEnv = xtiEnv;
    }

    public int? GetStatusCode()
    {
        int? statusCode = null;
        if (error != null)
        {
            if (error is AppException)
            {
                if (error is AccessDeniedException)
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
        }
        return statusCode;
    }

    public WebErrorResult GetErrorResult()
    {
        ErrorModel[] errors;
        AppEventSeverity severity;
        if (error == null)
        {
            errors = new[] { new ErrorModel(message, caption, "") };
            severity = AppEventSeverity.Values.CriticalError;
        }
        else
        {
            severity = new SeverityFromException(error).Value;
            if (error is ValidationFailedException validationFailedException)
            {
                errors = validationFailedException.Errors.ToArray();
            }
            else if (xtiEnv.IsDevelopmentOrTest())
            {
                errors = new[] { new ErrorModel(error.StackTrace ?? "", error.Message, "") };
            }
            else if (error is AppException appException)
            {
                errors = new[] { new ErrorModel(appException.DisplayMessage) };
            }
            else
            {
                errors = new[] { new ErrorModel("An unexpected error occurred") };
            }
        }
        return new WebErrorResult(logEntryKey, severity, errors);
    }

    public void Failed(Exception error, string logEntryKey)
    {
        string message;
        string caption;
        var xtiEnv = this.xtiEnv;
        if (error is AppException appError)
        {
            message = xtiEnv.IsProduction()
                ? appError.DisplayMessage
                : appError.ToString();
            caption = error is AccessDeniedException
                ? "Access Denied"
                : "Unexpected error";
        }
        else if (xtiEnv.IsProduction())
        {
            message = "An unexpected error occurred";
            caption = "Error";
        }
        else
        {
            message = error.ToString();
            caption = "An error occurred";
        }
        this.error = error;
        Failed(message, caption, logEntryKey);
    }

    public void Failed(string message, string caption, string logEntryKey)
    {
        this.message = message;
        this.caption = caption;
        this.logEntryKey = logEntryKey;
    }

    public bool HasError() =>
        error != null ||
        !string.IsNullOrWhiteSpace(message) ||
        !string.IsNullOrWhiteSpace(caption);

    public string Serialize()
    {
        var errorModel = new ErrorModel(message, caption, "");
        return XtiSerializer.Serialize(errorModel);
    }
}
