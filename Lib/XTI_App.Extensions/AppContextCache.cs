using Microsoft.Extensions.Caching.Memory;
using System;

namespace XTI_App.Extensions
{
    internal sealed class AppContextCache<T>
    {
        private readonly IMemoryCache cache;
        private readonly string cacheKey;

        public AppContextCache(IMemoryCache cache, string cacheKey)
        {
            this.cache = cache;
            this.cacheKey = cacheKey;
        }

        public T Get() => cache.Get<T>(cacheKey);

        public void Set(T data)
        {
            cache.Set
            (
                cacheKey,
                data,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(4, 0, 0)
                }
            );
        }
    }
}
