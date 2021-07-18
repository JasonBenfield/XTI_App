using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions
{
    public sealed class CachedUserContext : CachedUserContext<ISourceUserContext>
    {
        public CachedUserContext(IServiceProvider services, IMemoryCache cache)
            : base(services, cache)
        {
        }
    }

    public class CachedUserContext<TUserContext> : ICachedUserContext
        where TUserContext : ISourceUserContext
    {
        private readonly IServiceProvider services;
        private readonly IMemoryCache cache;

        public CachedUserContext(IServiceProvider services, IMemoryCache cache)
        {
            this.services = services;
            this.cache = cache;
        }

        public void ClearCache(int userID)
        {
            cache.Remove($"user_{userID}");
        }

        public void ClearCache(AppUserName userName)
        {
            cache.Remove($"user_{userName.Value}");
        }

        public async Task<IAppUser> User()
        {
            var userContext = services.GetService<TUserContext>();
            var userContextKey = await userContext.GetKey();
            var userKey = $"user_{userContextKey}";
            if (!cache.TryGetValue(userKey, out CachedAppUser cachedUser))
            {
                var user = await userContext.User();
                cachedUser = new CachedAppUser(services, user);
                cacheUser(userKey, cachedUser);
            }
            return cachedUser;
        }

        private void cacheUser(string key, CachedAppUser user)
        {
            cache.Set
            (
                key,
                user,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(1, 0, 0)
                }
            );
        }
    }
}
