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

        public DefaultUserContext(AppFactory appFactory, Func<int> getUserID)
        {
            this.appFactory = appFactory;
            this.getUserID = getUserID;
        }

        public Task<IAppUser> User() => User(getUserID());

        public async Task<IAppUser> User(int userID) => await appFactory.Users().User(userID);
    }
}
