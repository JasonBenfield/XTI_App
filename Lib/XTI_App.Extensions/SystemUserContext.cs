using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Secrets;

namespace XTI_App.Extensions
{
    public sealed class SystemUserContext : ISourceUserContext, ISystemUserContext
    {
        private readonly ISourceUserContext sourceUserContext;
        private readonly ISystemUserCredentials systemUserCredentials;

        public SystemUserContext(ISourceUserContext sourceUserContext, ISystemUserCredentials systemUserCredentials)
        {
            this.sourceUserContext = sourceUserContext;
            this.systemUserCredentials = systemUserCredentials;
        }

        public async Task<IAppUser> User()
        {
            var userName = await CurrentUserName();
            var user = await User(userName);
            return user;
        }

        public Task<IAppUser> User(AppUserName userName) => sourceUserContext.User(userName);

        public async Task<AppUserName> CurrentUserName()
        {
            var credentials = await systemUserCredentials.Value();
            return string.IsNullOrWhiteSpace(credentials.UserName)
                ? AppUserName.Anon
                : new AppUserName(credentials.UserName);
        }
    }
}
