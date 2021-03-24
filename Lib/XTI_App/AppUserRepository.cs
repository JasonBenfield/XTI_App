using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppUserRepository
    {
        private readonly AppFactory factory;

        public AppUserRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        public async Task<IEnumerable<AppUser>> Users()
        {
            var records = await factory.DB
                .Users
                .Retrieve()
                .OrderBy(u => u.UserName)
                .ToArrayAsync();
            return records.Select(u => factory.User(u));
        }

        public async Task<AppUser> User(int id)
        {
            var userRecord = await factory.DB
                .Users
                .Retrieve()
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
            => factory.DB
                .Users
                .Retrieve()
                .FirstOrDefaultAsync(u => u.UserName == userName.Value);

        public Task<AppUser> Add
        (
            AppUserName userName,
            IHashedPassword password,
            DateTimeOffset timeAdded
        ) => Add(userName, password, new PersonName(""), new EmailAddress(""), timeAdded);

        public async Task<AppUser> Add
        (
            AppUserName userName,
            IHashedPassword password,
            PersonName name,
            EmailAddress email,
            DateTimeOffset timeAdded
        )
        {
            var newUser = new AppUserRecord
            {
                UserName = userName.Value,
                Password = password.Value(),
                Name = name.Value,
                Email = email.Value,
                TimeAdded = timeAdded
            };
            await factory.DB.Users.Create(newUser);
            return factory.User(newUser);
        }
    }
}
