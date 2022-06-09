namespace XTI_WebApp.Abstractions;

public sealed class AppClientDomainSelector : IAppClientDomain
{
    private readonly List<Func<IAppClientDomain>> createAppClientDomains = new List<Func<IAppClientDomain>>();

    public void AddAppClientDomain(Func<IAppClientDomain> createAppClientDomain)
    {
        createAppClientDomains.Add(createAppClientDomain);
    }

    public async Task<string> Value(string appName, string version)
    {
        var domain = "";
        foreach (var createAppClientDomain in createAppClientDomains)
        {
            var appClientDomain = createAppClientDomain();
            domain = await appClientDomain.Value(appName, version);
            if (!string.IsNullOrWhiteSpace(domain))
            {
                break;
            }
        }
        return domain;
    }
}