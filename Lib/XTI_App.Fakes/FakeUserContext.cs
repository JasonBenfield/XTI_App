using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes
{
    public sealed class FakeUserContext : ISourceUserContext
    {
        private readonly FakeAppContext appContext;
        private AppUserName currentUserName;
        private readonly List<FakeAppUser> users = new List<FakeAppUser>();

        public FakeUserContext(FakeAppContext appContext)
        {
            this.appContext = appContext;
            AddUser(AppUserName.Anon);
            SetCurrentUser(AppUserName.Anon);
        }

        public Task<AppUserName> CurrentUserName() => Task.FromResult(currentUserName);

        public Task<IAppUser> User() => User(currentUserName);

        public Task<IAppUser> User(AppUserName userName)
        {
            var user = users.First(u => u.UserName().Equals(userName));
            return Task.FromResult<IAppUser>(user);
        }

        public void SetCurrentUser(AppUserName userName)
        {
            currentUserName = userName;
        }

        public FakeAppUser AddUser(AppUserName userName)
        {
            var user = new FakeAppUser(appContext, FakeAppUser.NextID(), userName);
            users.Add(user);
            return user;
        }
    }
}
