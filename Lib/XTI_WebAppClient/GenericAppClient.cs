namespace XTI_WebAppClient;

public sealed class GenericAppClient : AppClient
{
    public GenericAppClient(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, string appName, AppClientVersion version)
        : base(httpClientFactory, xtiTokenAccessor, clientUrl, appName, version.IsBlank() ? "Current" : version.Value)
    {
        UserCache = GetGroup
        (
            (_httpClientFactory, _tokenAccessor, _clientUrl) =>
                new UserCacheClientGroup(_httpClientFactory, _tokenAccessor, _clientUrl)
        );
    }

    public UserCacheClientGroup UserCache { get; }

    public GenericAppClientGroup Group(string name) =>
        GetGroup
        (
            (_httpClientFactory, _tokenAccessor, _clientUrl) =>
                new GenericAppClientGroup(_httpClientFactory, _tokenAccessor, _clientUrl, name)
        );

}