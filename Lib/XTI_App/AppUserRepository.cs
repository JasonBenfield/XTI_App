using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using XTI_App.Entities;
using XTI_Core;

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
            var record = await user(userName);
            if (record == null)
            {
                record = await user(AppUserName.Anon);
            }
            return factory.User(record);
        }

        private Task<AppUserRecord> user(AppUserName userName)
            => repo.Retrieve().FirstOrDefaultAsync(u => u.UserName == userName.Value);

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
