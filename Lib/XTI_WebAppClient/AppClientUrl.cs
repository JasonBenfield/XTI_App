﻿using XTI_WebApp.Abstractions;

namespace XTI_WebAppClient;

public sealed class AppClientUrl
{
    private readonly IAppClientDomain clientDomain;
    private readonly string appName;
    private readonly string version;
    private readonly string groupName;

    public AppClientUrl(IAppClientDomain clientDomain)
        : this(clientDomain, "", "", "")
    {
    }

    private AppClientUrl(IAppClientDomain clientDomain, string appName, string version, string groupName)
    {
        this.clientDomain = clientDomain;
        this.appName = appName;
        this.version = version;
        this.groupName = groupName;
    }

    public AppClientUrl WithApp(string appName, string version) =>
        new AppClientUrl(clientDomain, appName, version, groupName);

    public AppClientUrl WithGroup(string groupName) =>
        new AppClientUrl(clientDomain, appName, version, groupName);

    public async Task<string> Value(string actionName, string modifier)
    {
        var domain = await clientDomain.Value(appName, version);
        var url = $"https://{domain}/{appName}/{version}/{groupName}/{actionName}";
        if (!string.IsNullOrWhiteSpace(modifier))
        {
            url = $"{url}/{modifier}";
        }
        return url;
    }
}