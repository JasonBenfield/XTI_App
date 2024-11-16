using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class DefaultWebAppOptions : DefaultAppOptions
{
    public XtiCorsOptions XtiCors { get; set; } = new();
    public XtiAuthenticationOptions XtiAuthentication { get; set; } = new();
    public WebAppOptions WebApp { get; set; } = new();
    public AnonClientOptions AnonClient { get; set; } = new();
}
