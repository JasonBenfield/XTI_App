using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed class LoginUrl
{
    private readonly ILoginReturnKey returnKey;
    private readonly XtiAuthenticationOptions options;

    public LoginUrl(ILoginReturnKey returnKey, XtiAuthenticationOptions options)
    {
        this.returnKey = returnKey;
        this.options = options;
    }

    public async Task<string> Value(string returnUrl)
    {
        var returnKeyValue = await returnKey.Value(returnUrl);
        var delimiter = options.AuthenticatorUrl.Contains("?")
            ? "&"
            : "?";
        return $"{options.AuthenticatorUrl}{delimiter}returnKey={returnKeyValue}";
    }
}
