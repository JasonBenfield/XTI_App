namespace XTI_WebApp.Api;

public sealed class WebAppOptions
{
    public static readonly string WebApp = "WebApp";
    public string CacheBust { get; set; } = "";
}