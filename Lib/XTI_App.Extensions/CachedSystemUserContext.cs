using Microsoft.Extensions.Caching.Memory;
using System;

namespace XTI_App.Extensions
{
    public sealed class CachedSystemUserContext : CachedUserContext<SystemUserContext>
    {
        public CachedSystemUserContext(IServiceProvider services, IMemoryCache cache)
            : base(services, cache)
        {
        }
    }
}
