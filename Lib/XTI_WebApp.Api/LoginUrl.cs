using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed class LoginUrl
{
    private readonly ILoginReturnKey returnKey;
    private readonly DefaultWebAppOptions options;

    public LoginUrl(ILoginReturnKey returnKey, DefaultWebAppOptions options)
    {
        this.returnKey = returnKey;
        this.options = options;
    }

    public async Task<string> Value(string returnUrl)
    {
        var returnKeyValue = await returnKey.Value(returnUrl);
        var delimiter = options.XtiAuthentication.AuthenticatorUrl.Contains("?")
            ? "&"
            : "?";
        return $"{options.XtiAuthentication.AuthenticatorUrl}{delimiter}returnKey={returnKeyValue}";
    }
}
