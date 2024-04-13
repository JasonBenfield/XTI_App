using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class ClearCacheAction : AppAction<string, EmptyActionResult>
{
    private readonly ICachedUserContext cachedUserContext;

    public ClearCacheAction(ICachedUserContext cachedUserContext)
    {
        this.cachedUserContext = cachedUserContext;
    }

    public Task<EmptyActionResult> Execute(string userName, CancellationToken stoppingToken)
    {
        if (!string.IsNullOrWhiteSpace(userName))
        {
            cachedUserContext.ClearCache(new AppUserName(userName));
        }
        return Task.FromResult(new EmptyActionResult());
    }
}