using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.EfApi;

namespace XTI_App.Fakes
{
    public sealed class FakeUserContext : ISourceUserContext
    {
        private readonly DefaultUserContext userContext;

        private int userID;

        public FakeUserContext(AppFactory appFactory)
        {
            userContext = new DefaultUserContext(appFactory, getUserID);
        }

        private int getUserID() => userID;

        public Task<IAppUser> User() => userContext.User();

        public Task<string> GetKey() => Task.FromResult(getUserID().ToString());

        public void SetUser(IAppUser user)
        {
            userID = user?.ID.Value ?? 0;
        }

        public void RefreshUser(IAppUser user)
        {
        }
    }
}
