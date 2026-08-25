namespace XTI_WebApp.Abstractions;

public interface ILoginReturnKey
{
    Task<string> Value(string requesterKey, string returnUrl, CancellationToken ct);
}
