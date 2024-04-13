namespace XTI_WebApp.Abstractions;

public sealed class LogoutRequest
{
    public LogoutRequest()
        : this("")
    {
    }

    public LogoutRequest(string returnUrl)
    {
        ReturnUrl = returnUrl;
    }

    public string ReturnUrl { get; set; }
}
