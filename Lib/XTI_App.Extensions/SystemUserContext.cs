using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Secrets;

namespace XTI_App.Extensions
{
    public sealed class SystemUserContext : ISourceUserContext, ISystemUserContext
    {
        private readonly AppFactory appFactory;
        private readonly ISystemUserCredentials systemUserCredentials;

        public SystemUserContext(AppFactory appFactory, ISystemUserCredentials systemUserCredentials)
        {
            this.appFactory = appFactory;
            this.systemUserCredentials = systemUserCredentials;
        }

        public async Task<IAppUser> User()
        {
            var userName = await getUserName();
            var user = await appFactory.Users().User(new AppUserName(userName));
            return user;
        }

        public Task<string> GetKey() => getUserName();

        private async Task<string> getUserName()
        {
            var credentials = await systemUserCredentials.Value();
            return credentials.UserName;
        }
    }
}
