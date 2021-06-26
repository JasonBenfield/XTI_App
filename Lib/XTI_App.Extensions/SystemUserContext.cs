using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions
{
    public sealed class SystemUserContext : IUserContext
    {
        private readonly AppFactory appFactory;
        private readonly SystemUserCredentials systemUserCredentials;

        public SystemUserContext(AppFactory appFactory, SystemUserCredentials systemUserCredentials)
        {
            this.appFactory = appFactory;
            this.systemUserCredentials = systemUserCredentials;
        }

        public async Task<IAppUser> User()
        {
            var credentials = await systemUserCredentials.Value();
            var user = await appFactory.Users().User(new AppUserName(credentials.UserName));
            return user;
        }
    }
}
