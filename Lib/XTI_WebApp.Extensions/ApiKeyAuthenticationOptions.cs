namespace XTI_WebApp.Extensions;

public sealed class ApiKeyAuthenticationOptions
{
    public static readonly string ApiKeyAuth = nameof(ApiKeyAuth);

    public ApiKeyHeaderOptions[] Headers { get; set; } = new ApiKeyHeaderOptions[0];
}
