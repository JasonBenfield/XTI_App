using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.EfApi;

namespace XTI_App.Fakes
{
    public sealed class FakeUserContext : ISourceUserContext
    {
        private readonly DefaultUserContext userContext;

        private string userName;

        public FakeUserContext(AppFactory appFactory)
        {
            userContext = new DefaultUserContext(appFactory, getUserName);
        }

        private string getUserName() => userName;

        public Task<IAppUser> User() => userContext.User();

        public Task<string> GetKey() => Task.FromResult(getUserName().ToString());

        public void SetUser(IAppUser user)
        {
            userName = user?.UserName().Value ?? "";
        }
    }
}
