using Microsoft.Extensions.Caching.Memory;

namespace XTI_WebApp.Abstractions;

public sealed class AppClients
{
    private readonly IMemoryCache cache;
    private readonly List<AppVersionDomain> appVersions = new List<AppVersionDomain>();
    private readonly AppClientDomainSelector appClientDomains;

    public AppClients(IMemoryCache cache, AppClientDomainSelector appClientDomains)
    {
        this.cache = cache;
        this.appClientDomains = appClientDomains;
    }

    public void AddAppVersion(string appName, string version)
    {
        appVersions.Add(new AppVersionDomain(appName, version, ""));
    }

    public async Task<AppVersionDomain[]> Domains()
    {
        var appVersionDomains = new List<AppVersionDomain>();
        foreach (var appVersion in appVersions)
        {
            var domain = await GetDomain(appVersion);
            appVersionDomains.Add(appVersion with { Domain = domain });
        }
        return appVersionDomains.ToArray();
    }

    private async Task<string> GetDomain(AppVersionDomain appVersion)
    {
        var cacheKey = $"{appVersion.App}|{appVersion.Version}";
        if (!cache.TryGetValue<string>(cacheKey, out var domain))
        {
            domain = await appClientDomains.Value(appVersion.App, appVersion.Version);
            cache.Set(cacheKey, domain);
        }
        return domain;
    }
}