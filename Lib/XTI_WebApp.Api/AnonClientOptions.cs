namespace XTI_WebApp.Api;

public sealed class AnonClientOptions
{
    public const string AnonClient = nameof(AnonClient);

    public string CookieName { get; set; } = "";
    public string CookieDomain { get; set; } = "";
}