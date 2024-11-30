﻿using XTI_WebApp.Abstractions;

namespace XTI_WebAppClient;

public sealed class GenericAppClientFactory
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessorFactory xtiTokenAccessorFactory;
    private readonly AppClientUrl clientUrl;
    private readonly AppClientOptions options;

    public GenericAppClientFactory
    (
        IHttpClientFactory httpClientFactory,
        XtiTokenAccessorFactory xtiTokenAccessorFactory,
        AppClientUrl clientUrl,
        AppClientOptions options
    )
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessorFactory = xtiTokenAccessorFactory;
        this.clientUrl = clientUrl;
        this.options = options;
    }

    public GenericAppClient Create(string appName) => Create(appName, "Current");

    public GenericAppClient Create(string appName, string versionKey) =>
        new
        (
            httpClientFactory,
            xtiTokenAccessorFactory,
            clientUrl,
            options,
            appName,
            AppClientVersion.Version(versionKey)
        );

    public GenericAppClient Create(string appName, string versionKey, string domain) =>
        new
        (
            httpClientFactory,
            xtiTokenAccessorFactory,
            new AppClientUrl(new FixedAppClientDomain(domain)),
            options,
            appName,
            AppClientVersion.Version(versionKey)
        );
}
