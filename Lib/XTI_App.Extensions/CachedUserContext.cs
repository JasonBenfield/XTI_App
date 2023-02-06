using Microsoft.Extensions.Caching.Memory;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

public sealed class CachedUserContext : ICachedUserContext
{
    private readonly IMemoryCache cache;
    private readonly ISourceUserContext sourceUserContext;
    private readonly ICurrentUserName currentUserName;

    public CachedUserContext(IMemoryCache cache, ISourceUserContext sourceUserContext, ICurrentUserName currentUserName)
    {
        this.cache = cache;
        this.sourceUserContext = sourceUserContext;
        this.currentUserName = currentUserName;
    }

    public void ClearCache(AppUserName userName)
    {
        var cacheKey = $"xti_{userName.Value}";
        cache.Remove(cacheKey);
    }

    public async Task<UserContextModel> User()
    {
        var userName = await currentUserName.Value();
        var cachedUser = await User(userName);
        return cachedUser;
    }

    public async Task<UserContextModel> User(AppUserName userName)
    {
        var cacheKey = $"xti_{userName.Value}";
        if (!cache.TryGetValue<UserContextModel>(cacheKey, out var cachedUser))
        {
            cachedUser = await sourceUserContext.User(userName);
            cache.Set
            (
                cacheKey,
                cachedUser,
                TimeSpan.FromHours(1)
            );
        }
        return cachedUser ?? new UserContextModel();
    }
}