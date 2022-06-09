namespace XTI_WebAppClient;

public interface IAuthApiClientGroup
{
    public Task<ILoginResult> Authenticate(LoginCredentials model);
}