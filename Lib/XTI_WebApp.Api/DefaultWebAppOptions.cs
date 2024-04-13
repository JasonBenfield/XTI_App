using XTI_App.Api;
using XTI_Core;

namespace XTI_WebApp.Api;

public sealed class DefaultWebAppOptions
{
    public XtiCorsOptions XtiCors { get; set; } = new();
    public XtiAuthenticationOptions XtiAuthentication { get; set; } = new();
    public WebAppOptions WebApp { get; set; } = new();
    public AnonClientOptions AnonClient { get; set; } = new();
    public XtiTokenOptions XtiToken { get; set; } = new();
    public HubClientOptions HubClient { get; set; } = new();
    public DbOptions DB { get; set; } = new();
}
