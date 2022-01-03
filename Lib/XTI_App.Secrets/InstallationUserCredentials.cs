using XTI_Credentials;
using XTI_Secrets;

namespace XTI_App.Secrets;

public sealed class InstallationUserCredentials : IInstallationUserCredentials
{
    private static readonly string credentialKey = "Installation";

    private readonly ISecretCredentialsFactory secretCredentialsFactory;

    public InstallationUserCredentials(ISecretCredentialsFactory secretCredentialsFactory)
    {
        this.secretCredentialsFactory = secretCredentialsFactory;
    }

    public Task<CredentialValue> Value()
    {
        var secretCredentials = secretCredentialsFactory.Create(credentialKey);
        return secretCredentials.Value();
    }

    public Task Update(CredentialValue credentials)
    {
        var secretCredentials = secretCredentialsFactory.Create(credentialKey);
        return secretCredentials.Update(credentials);
    }
}