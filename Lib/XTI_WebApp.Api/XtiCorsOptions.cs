namespace XTI_WebApp.Api;

public sealed class XtiCorsOptions
{
    public static readonly string XtiCors = nameof(XtiCorsOptions);

    public string[] Origins { get; set; } = new string[0];
}
