﻿using System.Threading.Tasks;
using XTI_App.Api;

namespace XTI_App.Fakes
{
    public sealed class FakeUserContext : IUserContext
    {
        private AppUser user;

        public Task<IAppUser> User() => Task.FromResult((IAppUser)user);

        public Task<AppUser> UncachedUser() => Task.FromResult(user);

        public void SetUser(AppUser user) => this.user = user;

        public void RefreshUser(IAppUser user)
        {
        }
    }
}
