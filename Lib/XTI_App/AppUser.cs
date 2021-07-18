﻿using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class AppUser : IAppUser
    {
        private readonly AppFactory factory;
        private readonly AppUserRecord record;

        internal AppUser(AppFactory factory, AppUserRecord record)
        {
            this.factory = factory;
            this.record = record ?? new AppUserRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public AppUserName UserName() => new AppUserName(record.UserName);
        public bool Exists() => ID.IsValid() && !UserName().Equals(AppUserName.Anon);

        public bool IsPasswordCorrect(IHashedPassword hashedPassword) =>
            hashedPassword.Equals(record.Password);

        public async Task AddRole(AppRole role)
        {
            var app = await role.App();
            var modifier = await app.DefaultModifier();
            await AddRole(role, modifier);
        }

        public Task AddRole(AppRole role, Modifier modifier)
        {
            var record = new AppUserRoleRecord
            {
                UserID = ID.Value,
                RoleID = role.ID.Value,
                ModifierID = modifier.ID.Value
            };
            return factory.DB.UserRoles.Create(record);
        }

        public async Task RemoveRole(AppRole role)
        {
            var userRole = await factory.DB
                .UserRoles
                .Retrieve()
                .Where(ur => ur.UserID == ID.Value && ur.RoleID == role.ID.Value)
                .FirstOrDefaultAsync();
            if (userRole != null)
            {
                await factory.DB.UserRoles.Delete(userRole);
            }
        }

        async Task<IEnumerable<IAppRole>> IAppUser.Roles(IApp app, IModifier modifier)
            => await AssignedRoles(app, modifier);

        public Task<AppRole[]> ExplicitlyUnassignedRoles(App app, Modifier modifier)
            => factory.Roles().RolesNotAssignedToUser(this, app, modifier);

        public async Task<AppRole[]> AssignedRoles(IApp app, IModifier modifier)
        {
            var roles = await ExplicitlyAssignedRoles(app, modifier);
            if (!roles.Any() && !modifier.ModKey().Equals(ModifierKey.Default))
            {
                var defaultModCategory = await app.ModCategory(ModifierCategoryName.Default);
                var defaultModifier = await defaultModCategory.Modifier(ModifierKey.Default);
                roles = await ExplicitlyAssignedRoles(app, defaultModifier);
            }
            return roles;
        }

        public Task<AppRole[]> ExplicitlyAssignedRoles(IApp app, IModifier modifier)
            => factory.Roles().RolesAssignedToUser(this, app, modifier);

        public Task ChangePassword(IHashedPassword password)
            => factory.DB.Users.Update(record, u => u.Password = password.Value());

        public Task Edit(PersonName name, EmailAddress email)
            => factory.DB.Users.Update
            (
                record,
                u =>
                {
                    u.Name = name.Value;
                    u.Email = email.Value;
                }
            );

        public AppUserModel ToModel() => new AppUserModel
        {
            ID = ID.Value,
            UserName = UserName().DisplayText,
            Name = new PersonName(record.Name).DisplayText,
            Email = new EmailAddress(record.Email).DisplayText
        };

        public override string ToString() => $"{nameof(AppUser)} {ID.Value}";

    }
}
