namespace XTI_WebApp.Api;

public sealed class XtiAuthenticationOptions
{
    public const string XtiAuthentication = nameof(XtiAuthentication);

    public string AuthenticatorUrl { get; set; } = "";
    public string JwtSecret { get; set; } = "";
    public string CookieName { get; set; } = "";
    public string CookieDomain { get; set; } = "";
}