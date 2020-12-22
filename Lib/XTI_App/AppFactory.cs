using MainDB.Entities;

namespace XTI_App
{
    public sealed class AppFactory
    {
        private readonly IMainDataRepositoryFactory repos;

        public AppFactory(IMainDataRepositoryFactory repos)
        {
            this.repos = repos;
        }

        private AppUserRepository users;
        public AppUserRepository Users()
            => users ?? (users = new AppUserRepository(this, repos.CreateUsers()));
        internal AppUser User(AppUserRecord record) => new AppUser(repos.CreateUsers(), this, record);

        private AppRepository apps;
        public AppRepository Apps()
            => apps ?? (apps = new AppRepository(this, repos.CreateApps()));
        internal App App(AppRecord record) =>
            new App(repos.CreateApps(), this, record);

        private AppVersionRepository versions;
        public AppVersionRepository Versions()
            => versions ?? (versions = new AppVersionRepository(this, repos.CreateVersions()));
        internal AppVersion Version(AppVersionRecord record) => new AppVersion(this, repos.CreateVersions(), record);

        private AppRoleRepository roles;
        internal AppRoleRepository Roles()
            => roles ?? (roles = new AppRoleRepository(this, repos.CreateRoles()));
        internal AppRole Role(AppRoleRecord record) => new AppRole(repos.CreateRoles(), record);

        private AppUserRoleRepository userRoles;
        internal AppUserRoleRepository UserRoles()
            => userRoles ?? (userRoles = new AppUserRoleRepository(this, repos.CreateUserRoles()));
        internal AppUserRole UserRole(AppUserRoleRecord record) =>
            new AppUserRole(repos.CreateUserRoles(), record);

        private ResourceGroupRepository groups;
        internal ResourceGroupRepository Groups()
            => groups ?? (groups = new ResourceGroupRepository(this, repos.CreateResourceGroups()));
        internal ResourceGroup Group(ResourceGroupRecord record)
            => new ResourceGroup(repos.CreateResourceGroups(), this, record);

        private ResourceRepository resources;
        internal ResourceRepository Resources()
            => resources ?? (resources = new ResourceRepository(this, repos.CreateResources()));
        internal Resource Resource(ResourceRecord record) => new Resource(record);

        private ModifierCategoryRepository modCategories;
        public ModifierCategoryRepository ModCategories()
            => modCategories
                ?? (modCategories = new ModifierCategoryRepository(this, repos.CreateModifierCategories()));
        internal ModifierCategory ModCategory(ModifierCategoryRecord record) => new ModifierCategory(this, record);

        private ModifierRepository modifiers;
        internal ModifierRepository Modifiers()
            => modifiers ?? (modifiers = new ModifierRepository(this, repos.CreateModifiers()));
        internal Modifier Modifier(ModifierRecord record)
            => new Modifier(repos.CreateModifiers(), record);

        private ModifierCategoryAdminRepository modCategoryAdmins;
        internal ModifierCategoryAdminRepository ModCategoryAdmins()
            => modCategoryAdmins
                ?? (modCategoryAdmins = new ModifierCategoryAdminRepository(this, repos.CreateModifierCategoryAdmins()));

        private AppUserModifierRepository userModifiers;
        internal AppUserModifierRepository UserModifiers()
            => userModifiers
                ?? (userModifiers = new AppUserModifierRepository(this, repos.CreateUserModifiers()));
        internal AppUserModifier UserModifier(AppUserModifierRecord record) => new AppUserModifier(this, record);
    }
}
