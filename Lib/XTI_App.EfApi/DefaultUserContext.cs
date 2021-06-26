using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.EfApi
{
    public sealed class DefaultUserContext : IUserContext
    {
        private readonly AppFactory appFactory;
        private readonly Func<int> getUserID;

        public DefaultUserContext(AppFactory appFactory, int userID)
            : this(appFactory, () => userID)
        {
        }

        public DefaultUserContext(AppFactory appFactory, Func<int> getUserID)
        {
            this.appFactory = appFactory;
            this.getUserID = getUserID;
        }

        public async Task<IAppUser> User() => await appFactory.Users().User(getUserID());
    }
}
