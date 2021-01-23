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

        public Task<IEnumerable<AppUserRole>> RolesForApp(IApp app) =>
            factory.UserRoles().RolesForUser(this, app);

        async Task<IEnumerable<IAppUserRole>> IAppUser.RolesForApp(IApp app) =>
            await RolesForApp(app);

        public Task RemoveRole(AppUserRole userRole) => userRole.Delete();

        public Task ChangePassword(IHashedPassword password)
            => repo.Update(record, u => u.Password = password.Value());

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
            var record = await userModRepo.Retrieve()
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

        public override string ToString() => $"{nameof(AppUser)} {ID.Value}";

    }
}
