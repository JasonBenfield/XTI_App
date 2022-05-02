using Microsoft.Extensions.Caching.Memory;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

public sealed class CachedUserContext : ICachedUserContext
{
    private readonly IMemoryCache cache;
    private readonly ISourceAppContext sourceAppContext;
    private readonly ISourceUserContext sourceUserContext;

    public CachedUserContext(IMemoryCache cache, ISourceAppContext sourceAppContext, ISourceUserContext sourceUserContext)
    {
        this.cache = cache;
        this.sourceAppContext = sourceAppContext;
        this.sourceUserContext = sourceUserContext;
    }

    public void ClearCache(AppUserName userName)
    {
        var cachedUser = new CachedAppUser(cache, sourceAppContext, sourceUserContext, userName);
        cachedUser.ClearCache();
    }

    public async Task<IAppUser> User()
    {
        var userName = await sourceUserContext.CurrentUserName();
        var cachedUser = await User(userName);
        return cachedUser;
    }

    public async Task<IAppUser> User(AppUserName userName)
    {
        var cachedUser = new CachedAppUser(cache, sourceAppContext, sourceUserContext, userName);
        await cachedUser.Load();
        return cachedUser;
    }
}