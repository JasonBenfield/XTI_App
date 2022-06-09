using XTI_Credentials;

namespace XTI_WebAppClient;

public class AuthenticatorXtiToken : IXtiToken
{
    private readonly IAuthClient authClient;
    private readonly ICredentials credentials;

    public AuthenticatorXtiToken(IAuthClient authClient, ICredentials credentials)
    {
        this.authClient = authClient;
        this.credentials = credentials;
    }

    public async Task<string> Value()
    {
        var credentialsValue = await credentials.Value();
        var loginModel = new LoginCredentials
        {
            UserName = credentialsValue.UserName,
            Password = credentialsValue.Password
        };
        var result = await authClient.AuthApi.Authenticate(loginModel);
        return result.Token;
    }
}