using Microsoft.Extensions.Caching.Memory;
using XTI_App.Api;

namespace XTI_App.Extensions
{
    public sealed class CachedSystemUserContext : CachedUserContext<SystemUserContext>, ISystemUserContext
    {
        public CachedSystemUserContext(IMemoryCache cache, AppFactory appFactory, SystemUserContext sourceUserContext)
            : base(cache, appFactory, sourceUserContext)
        {
        }
    }
}
