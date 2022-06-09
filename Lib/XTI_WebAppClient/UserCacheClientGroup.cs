namespace XTI_WebAppClient;

public sealed class UserCacheClientGroup : AppClientGroup
{
    public UserCacheClientGroup(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl)
        : base(httpClientFactory, xtiTokenAccessor, clientUrl, "UserCache")
    {
    }

    public Task<EmptyActionResult> ClearCache(ClearUserCacheRequest request)
        => Post<EmptyActionResult, ClearUserCacheRequest>(nameof(ClearCache), "", request);
}