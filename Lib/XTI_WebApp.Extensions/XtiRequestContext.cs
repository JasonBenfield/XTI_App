using XTI_App.Api;
using XTI_Core;

namespace XTI_WebApp.Extensions;

public sealed class XtiRequestContext
{
    private readonly XtiEnvironment xtiEnvAccessor;
    private Exception? error;

    public XtiRequestContext(XtiEnvironment xtiEnvAccessor)
    {
        this.xtiEnvAccessor = xtiEnvAccessor;
    }

    public void Failed(Exception error) => this.error = error;

    public bool HasError() => error != null;

    public Exception Error() => error ?? throw new ArgumentNullException(nameof(error));

    public string Serialize()
    {
        var errorModel = ToModel();
        return XtiSerializer.Serialize(errorModel);
    }

    private ErrorModel ToModel()
    {
        string message;
        string caption;
        if (HasError())
        {
            var error = Error();
            var xtiEnv = xtiEnvAccessor;
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
        }
        else
        {
            message = "";
            caption = "";
        }
        return new ErrorModel(message, caption, "");
    }
}
