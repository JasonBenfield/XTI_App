using Microsoft.Extensions.Caching.Memory;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

public sealed class CachedAppContext : IAppContext
{
    private readonly IMemoryCache cache;
    private readonly ISourceAppContext sourceAppContext;
    private readonly AppKey appKey;
    private readonly AppVersionKey versionKey;

    public CachedAppContext(IMemoryCache cache, ISourceAppContext sourceAppContext, AppKey appKey, AppVersionKey versionKey)
    {
        this.cache = cache;
        this.sourceAppContext = sourceAppContext;
        this.appKey = appKey;
        this.versionKey = versionKey;
    }

    public async Task<IApp> App()
    {
        var cachedApp = new CachedApp(cache, sourceAppContext, appKey);
        await cachedApp.Load();
        return cachedApp;
    }

    public async Task<IAppVersion> Version()
    {
        var app = await App();
        var version = await app.Version(versionKey);
        return version;
    }
}