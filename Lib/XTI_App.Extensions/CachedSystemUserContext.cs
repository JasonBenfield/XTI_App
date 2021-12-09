using Microsoft.Extensions.Caching.Memory;
using XTI_App.Api;

namespace XTI_App.Extensions;

public sealed class CachedSystemUserContext : CachedUserContext<SystemUserContext>, ISystemUserContext
{
    public CachedSystemUserContext(IMemoryCache cache, ISourceAppContext sourceAppContext, SystemUserContext sourceUserContext)
        : base(cache, sourceAppContext, sourceUserContext)
    {
    }
}