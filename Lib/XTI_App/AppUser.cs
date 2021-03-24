using MainDB.Entities;
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

        public Task AddRole(AppRole role)
        {
            var record = new AppUserRoleRecord
            {
                UserID = ID.Value,
                RoleID = role.ID.Value
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

        async Task<IEnumerable<IAppRole>> IAppUser.Roles(IApp app) => await AssignedRoles(app);

        public Task<AppRole[]> UnassignedRoles(IApp app)
            => factory.Roles().RolesNotAssignedToUser(this, app);

        public Task<AppRole[]> AssignedRoles(IApp app)
            => factory.Roles().RolesAssignedToUser(this, app);

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

        public Task<bool> IsModCategoryAdmin(IModifierCategory modCategory)
            => factory.ModCategoryAdmins().IsAdmin(modCategory, this);

        public Task GrantFullAccessToModCategory(ModifierCategory modCategory)
        {
            return factory.DB.Transaction(async () =>
            {
                await factory.ModCategoryAdmins().Add(modCategory, this);
                var userModifiers = await factory.DB
                    .UserModifiers
                    .Retrieve()
                    .Where(um => um.UserID == ID.Value)
                    .ToArrayAsync();
                foreach (var userModifier in userModifiers)
                {
                    await factory.DB.UserModifiers.Delete(userModifier);
                }
            });
        }

        public Task RevokeFullAccessToModCategory(ModifierCategory modCategory)
            => factory.ModCategoryAdmins().Delete(modCategory, this);

        public Task AddModifier(Modifier modifier)
        {
            var record = new AppUserModifierRecord
            {
                UserID = ID.Value,
                ModifierID = modifier.ID.Value
            };
            return factory.DB.UserModifiers.Create(record);
        }

        public async Task RemoveModifier(Modifier modifier)
        {
            var record = await factory.DB
                .UserModifiers
                .Retrieve()
                .FirstOrDefaultAsync
                (
                    um => um.UserID == ID.Value && um.ModifierID == modifier.ID.Value
                );
            if (record != null)
            {
                await factory.DB.UserModifiers.Delete(record);
            }
        }

        public Task<bool> HasModifier(ModifierKey modKey)
        {
            var modifierIDs = factory.DB
                .Modifiers
                .Retrieve()
                .Where(m => m.ModKey == modKey.Value)
                .Select(m => m.ID);
            return factory.DB
                .UserModifiers
                .Retrieve()
                .AnyAsync(um => um.UserID == ID.Value && modifierIDs.Any(id => id == um.ModifierID));
        }

        public Task<IEnumerable<Modifier>> UnassignedModifiers(ModifierCategory modCategory)
            => factory.Modifiers().ModifiersNotAssignedToUser(this, modCategory);

        public Task<IEnumerable<Modifier>> AssignedModifiers(ModifierCategory modCategory)
            => factory.Modifiers().ModifiersAssignedToUser(this, modCategory);

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
