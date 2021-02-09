using System.Collections.Generic;
using System.Threading.Tasks;
using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppUser : IAppUser
    {
        private readonly IMainDataRepositoryFactory repoFactory;
        private readonly DataRepository<AppUserRecord> repo;
        private readonly AppFactory factory;
        private readonly AppUserRecord record;

        internal AppUser(IMainDataRepositoryFactory repoFactory, AppFactory factory, AppUserRecord record)
        {
            this.repoFactory = repoFactory;
            this.repo = repoFactory.CreateUsers();
            this.factory = factory;
            this.record = record ?? new AppUserRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public AppUserName UserName() => new AppUserName(record.UserName);
        public bool Exists() => ID.IsValid() && !UserName().Equals(AppUserName.Anon);

        public bool IsPasswordCorrect(IHashedPassword hashedPassword) =>
            hashedPassword.Equals(record.Password);

        public Task<AppUserRole> AddRole(AppRole role) =>
            factory.UserRoles().Add(this, role);

        public Task RemoveRole(AppUserRole userRole) => userRole.Delete();

        public Task<AppRole[]> Roles(IApp app) =>
            factory.Roles().RolesForUser(this, app);

        async Task<IEnumerable<IAppRole>> IAppUser.Roles(IApp app) => await Roles(app);

        public Task<AppRole[]> UnassignedRoles(App app)
            => factory.Roles().RolesNotAssignedToUser(this, app);

        public Task<AppUserRoleModel[]> AssignedRoles(App app) 
            => factory.UserRoles().AssignedRoles(this, app);

        public Task ChangePassword(IHashedPassword password)
            => repo.Update(record, u => u.Password = password.Value());

        public Task Edit(PersonName name, EmailAddress email)
            => repo.Update
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
            => factory.ModCategoryAdmins().Add(modCategory, this);

        public Task RevokeFullAccessToModCategory(ModifierCategory modCategory)
            => factory.ModCategoryAdmins().Delete(modCategory, this);

        public Task AddModifier(Modifier modifier)
        {
            var record = new AppUserModifierRecord
            {
                UserID = ID.Value,
                ModifierID = modifier.ID.Value
            };
            return repoFactory.CreateUserModifiers().Create(record);
        }

        public async Task RemoveModifier(Modifier modifier)
        {
            var userModRepo = repoFactory.CreateUserModifiers();
            var record = await userModRepo
                .Retrieve()
                .FirstOrDefaultAsync
                (
                    um => um.UserID == ID.Value && um.ModifierID == modifier.ID.Value
                );
            if (record != null)
            {
                await userModRepo.Delete(record);
            }
        }

        public async Task<bool> HasModifier(ModifierKey modKey)
        {
            var modifier = await factory.Modifiers().Modifier(modKey);
            var hasModifier = await repoFactory
                .CreateUserModifiers()
                .Retrieve()
                .AnyAsync(um => um.UserID == ID.Value && um.ModifierID == modifier.ID.Value);
            return hasModifier;
        }

        public Task<IEnumerable<Modifier>> Modifiers(ModifierCategory modCategory)
            => factory.Modifiers().ModifiersForUser(this, modCategory);

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
