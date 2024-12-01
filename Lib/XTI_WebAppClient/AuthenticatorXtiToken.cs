using XTI_Credentials;

namespace XTI_WebAppClient;

public class AuthenticatorXtiToken : IXtiToken
{
    private readonly IAuthClient authClient;
    private readonly ICredentials credentials;
    private bool isInProgress;

    public AuthenticatorXtiToken(IAuthClient authClient, ICredentials credentials)
    {
        this.authClient = authClient;
        this.credentials = credentials;
    }

    public async Task<string> Value(CancellationToken ct)
    {
        var maxTime = DateTime.UtcNow.AddSeconds(30);
        while (DateTime.UtcNow < maxTime && isInProgress)
        {
            await Task.Delay(100, ct);
        }
        string token;
        try
        {
            isInProgress = true;
            var credentialsValue = await credentials.Value();
            var authRequest = new AuthenticateRequest
            {
                UserName = credentialsValue.UserName,
                Password = credentialsValue.Password
            };
            var result = await authClient.AuthApi.Authenticate(authRequest, ct);
            token = result.Token;
        }
        finally
        {
            isInProgress = false;
        }
        return token;
    }
}