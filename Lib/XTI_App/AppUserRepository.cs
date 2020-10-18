using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace XTI_App
{
    public sealed class AppUserRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppUserRecord> repo;

        public AppUserRepository(AppFactory factory, DataRepository<AppUserRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        public async Task<AppUser> User(int id)
        {
            var userRecord = await repo.Retrieve()
                .FirstOrDefaultAsync(u => u.ID == id);
            return factory.User(userRecord);
        }

        public async Task<AppUser> User(AppUserName userName)
        {
            var userRecord = await repo.Retrieve()
                .FirstOrDefaultAsync(u => u.UserName == userName.Value);
            return factory.User(userRecord);
        }

        public async Task<AppUser> Add(AppUserName userName, IHashedPassword password, DateTime timeAdded)
        {
            var newUser = new AppUserRecord
            {
                UserName = userName.Value,
                Password = password.Value(),
                TimeAdded = timeAdded
            };
            await repo.Create(newUser);
            return factory.User(newUser);
        }
    }
}
