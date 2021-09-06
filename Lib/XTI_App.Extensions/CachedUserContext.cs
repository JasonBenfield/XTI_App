using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions
{
    public sealed class CachedUserContext : CachedUserContext<ISourceUserContext>
    {
        public CachedUserContext(IMemoryCache cache, ISourceAppContext sourceAppContext, ISourceUserContext sourceUserContext)
            : base(cache, sourceAppContext, sourceUserContext)
        {
        }
    }

    public class CachedUserContext<TUserContext> : ICachedUserContext
        where TUserContext : ISourceUserContext
    {
        private readonly IMemoryCache cache;
        private readonly ISourceAppContext sourceAppContext;
        private readonly ISourceUserContext sourceUserContext;

        public CachedUserContext(IMemoryCache cache, ISourceAppContext sourceAppContext, TUserContext sourceUserContext)
        {
            this.cache = cache;
            this.sourceAppContext = sourceAppContext;
            this.sourceUserContext = sourceUserContext;
        }

        public void ClearCache(AppUserName userName)
        {
            var cachedUser = new CachedAppUser(cache, sourceAppContext, sourceUserContext);
            cachedUser.ClearCache();
        }

        public async Task<IAppUser> User()
        {
            var cachedUser = new CachedAppUser(cache, sourceAppContext, sourceUserContext);
            await cachedUser.Load();
            return cachedUser;
        }
    }
}
