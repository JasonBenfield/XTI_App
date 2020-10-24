using System.Threading.Tasks;
using XTI_App.Api;

namespace XTI_App.Fakes
{
    public sealed class FakeUserContext : IUserContext
    {
        private IAppUser user;

        public Task<IAppUser> User() => Task.FromResult(user);

        public Task<IAppUser> User(int id) => Task.FromResult<IAppUser>(null);

        public void SetUser(IAppUser user) => this.user = user;

        public void RefreshUser(IAppUser user)
        {
        }

    }
}
