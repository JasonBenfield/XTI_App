namespace XTI_WebApp.Abstractions;

public sealed class FixedAppClientDomain : IAppClientDomain
{
    private readonly string domain;

    public FixedAppClientDomain(string domain)
    {
        this.domain = domain;
    }

    public Task<string> Value(string appName, string version) => Task.FromResult(domain);
}
