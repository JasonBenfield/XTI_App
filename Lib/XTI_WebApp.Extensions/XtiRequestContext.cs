using Microsoft.AspNetCore.Http;
using XTI_App.Api;
using XTI_Core;

namespace XTI_WebApp.Extensions;

public sealed class XtiRequestContext
{
    private readonly XtiEnvironment xtiEnv;
    private Exception? error;
    private string message = "";
    private string caption = "";

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

    public ErrorModel[] GetErrors()
    {
        ErrorModel[] errors;
        if(error == null)
        {
            errors = new[] { new ErrorModel(message, caption, "") };
        }
        else
        {
            if (xtiEnv.IsDevelopmentOrTest())
            {
                if (error is ValidationFailedException validationFailedException)
                {
                    errors = validationFailedException.Errors.ToArray();
                }
                else
                {
                    errors = new[] { new ErrorModel(error.StackTrace ?? "", error.Message, "") };
                }
            }
            else if (error is AppException appException)
            {
                if (error is ValidationFailedException validationFailedException)
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
        }
        return errors;
    }

    public void Failed(Exception error)
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
        Failed(message, caption);
    }

    public void Failed(string message, string caption)
    {
        this.message = message;
        this.caption = caption;
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
