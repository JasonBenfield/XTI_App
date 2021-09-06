using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions
{
    public sealed class CachedUserContext : CachedUserContext<ISourceUserContext>
    {
        public CachedUserContext(IMemoryCache cache, AppFactory appFactory, ISourceUserContext sourceUserContext)
            : base(cache, appFactory, sourceUserContext)
        {
        }
    }

    public class CachedUserContext<TUserContext> : ICachedUserContext
        where TUserContext : ISourceUserContext
    {
        private readonly IMemoryCache cache;
        private readonly AppFactory appFactory;
        private readonly ISourceUserContext sourceUserContext;

        public CachedUserContext(IMemoryCache cache, AppFactory appFactory, TUserContext sourceUserContext)
        {
            this.cache = cache;
            this.appFactory = appFactory;
            this.sourceUserContext = sourceUserContext;
        }

        public void ClearCache(AppUserName userName)
        {
            var cachedUser = new CachedAppUser(cache, appFactory, userName);
            cachedUser.ClearCache();
        }

        public async Task<IAppUser> User()
        {
            var userContextKey = await sourceUserContext.GetKey();
            var cachedUser = new CachedAppUser(cache, appFactory, new AppUserName(userContextKey));
            await cachedUser.Load();
            return cachedUser;
        }
    }
}
