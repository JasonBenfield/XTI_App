namespace XTI_WebAppClient;

public interface IAuthApiClientGroup
{
    public Task<LoginResult> Authenticate(AuthenticateRequest authRequest, CancellationToken ct);
}