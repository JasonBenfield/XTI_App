using System.Collections.Generic;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppUser : IAppUser
    {
        private readonly DataRepository<AppUserRecord> repo;
        private readonly AppFactory factory;
        private readonly AppUserRecord record;

        internal AppUser(DataRepository<AppUserRecord> repo, AppFactory factory, AppUserRecord record)
        {
            this.repo = repo;
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

        public Task<IEnumerable<AppUserRole>> RolesForApp(App app) =>
            factory.UserRoles().RolesForUser(this, app);

        async Task<IEnumerable<IAppUserRole>> IAppUser.RolesForApp(IApp app) =>
            await factory.UserRoles().RolesForUser(this, app);

        public Task RemoveRole(AppUserRole userAdminRole) => userAdminRole.Delete();

        public Task ChangePassword(IHashedPassword password)
            => repo.Update(record, u => u.Password = password.Value());

        public Task<bool> IsModCategoryAdmin(IModifierCategory modCategory)
            => factory.ModCategoryAdmins().IsAdmin(modCategory, this);

        public Task GrantFullAccessToModCategory(ModifierCategory modCategory)
            => factory.ModCategoryAdmins().Add(modCategory, this);

        public Task RevokeFullAccessToModCategory(ModifierCategory modCategory)
            => factory.ModCategoryAdmins().Delete(modCategory, this);

        public Task AddModifier(Modifier modifier) => factory.UserModifiers().Add(this, modifier);

        public Task RemoveModifier(Modifier modifier) => factory.UserModifiers().Delete(this, modifier);

        public async Task<bool> HasModifier(ModifierKey modKey)
        {
            var modifier = await factory.Modifiers().Modifier(modKey);
            var hasModifier = await factory.UserModifiers().Any(this, modifier);
            return hasModifier;
        }

        public Task<IEnumerable<Modifier>> Modifiers(ModifierCategory modCategory)
            => factory.UserModifiers().Modifiers(this, modCategory);

        public override string ToString() => $"{nameof(AppUser)} {ID.Value}";

    }
}
