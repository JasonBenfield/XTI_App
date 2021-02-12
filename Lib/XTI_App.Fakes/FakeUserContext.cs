using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes
{
    public sealed class FakeUserContext : IUserContext
    {
        private readonly AppFactory appFactory;

        private int userID;

        public FakeUserContext(AppFactory appFactory)
        {
            this.appFactory = appFactory;
        }

        public Task<IAppUser> User() => User(userID);

        public async Task<IAppUser> User(int userID)
        {
            var user = await appFactory.Users().User(userID);
            if (!user.Exists())
            {
                user = await appFactory.Users().User(AppUserName.Anon);
            }
            return user;
        }

        public void SetUser(IAppUser user)
        {
            userID = user?.ID.Value ?? 0;
        }

        public void RefreshUser(IAppUser user)
        {
        }
    }
}
