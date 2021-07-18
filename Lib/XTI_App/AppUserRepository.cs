using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class AppUserRepository
    {
        private readonly AppFactory factory;

        public AppUserRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        public Task<AppUser[]> Users()
            => factory.DB
                .Users
                .Retrieve()
                .OrderBy(u => u.UserName)
                .Select(u => factory.User(u))
                .ToArrayAsync();

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

        public Task<AppUser[]> SystemUsers()
            => factory.DB
            .Users
            .Retrieve()
            .Where(u => u.UserName.StartsWith("xti_sys_"))
            .Select(u => factory.User(u))
            .ToArrayAsync();

        public Task<AppUser[]> SystemUsers(AppKey appKey)
            => factory.DB
            .Users
            .Retrieve()
            .Where(u => u.UserName.StartsWith(AppUserName.SystemUser(appKey, "").Value))
            .Select(u => factory.User(u))
            .ToArrayAsync();

        public Task<AppUser> SystemUser(AppKey appKey, string machineName)
            => User(AppUserName.SystemUser(appKey, machineName));

        private Task<AppUserRecord> user(AppUserName userName)
            => factory.DB
                .Users
                .Retrieve()
                .FirstOrDefaultAsync(u => u.UserName == userName.Value);

        public async Task<AppUser> AddSystemUser
        (
            AppKey appKey,
            string machineName,
            IHashedPassword password,
            DateTimeOffset timeAdded
        )
        {
            var user = await Add
            (
                AppUserName.SystemUser(appKey, machineName),
                password,
                new PersonName($"{appKey.Name.DisplayText.Replace(" ", "")} {appKey.Type.DisplayText.Replace(" ", "")} {machineName}"),
                new EmailAddress(""),
                timeAdded
            );
            var app = await factory.Apps().App(appKey);
            var role = await app.Role(AppRoleName.System);
            await user.AddRole(role);
            return user;
        }

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
