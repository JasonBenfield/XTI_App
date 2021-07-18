using MainDB.Entities;
using System;
using System.Threading.Tasks;

namespace XTI_App
{
    public sealed class AppFactory
    {
        public AppFactory(IMainDbContext db)
        {
            DB = db;
        }

        internal IMainDbContext DB { get; }

        private AppUserRepository users;
        public AppUserRepository Users()
            => users ?? (users = new AppUserRepository(this));
        internal AppUser User(AppUserRecord record) => new AppUser(this, record);

        private AppRepository apps;
        public AppRepository Apps()
            => apps ?? (apps = new AppRepository(this));
        internal App App(AppRecord record) =>
            new App(this, record);

        private AppVersionRepository versions;
        public AppVersionRepository Versions()
            => versions ?? (versions = new AppVersionRepository(this));
        internal AppVersion Version(AppVersionRecord record) => new AppVersion(this, record);

        private AppRoleRepository roles;
        internal AppRoleRepository Roles()
            => roles ?? (roles = new AppRoleRepository(this));
        internal AppRole Role(AppRoleRecord record) => new AppRole(this, record);

        private ResourceGroupRepository groups;
        internal ResourceGroupRepository Groups()
            => groups ?? (groups = new ResourceGroupRepository(this));
        internal ResourceGroup Group(ResourceGroupRecord record)
            => new ResourceGroup(this, record);

        private ResourceRepository resources;
        internal ResourceRepository Resources()
            => resources ?? (resources = new ResourceRepository(this));
        internal Resource Resource(ResourceRecord record) => new Resource(this, record);

        private ModifierCategoryRepository modCategories;
        public ModifierCategoryRepository ModCategories()
            => modCategories
                ?? (modCategories = new ModifierCategoryRepository(this));
        internal ModifierCategory ModCategory(ModifierCategoryRecord record) => new ModifierCategory(this, record);

        private ModifierRepository modifiers;
        internal ModifierRepository Modifiers()
            => modifiers ?? (modifiers = new ModifierRepository(this));
        internal Modifier Modifier(ModifierRecord record)
            => new Modifier(this, record);

        private AppSessionRepository sessions;
        public AppSessionRepository Sessions()
            => sessions ?? (sessions = new AppSessionRepository(this));
        internal AppSession Session(AppSessionRecord record) =>
            new AppSession(this, record);

        private AppRequestRepository requests;
        public AppRequestRepository Requests()
            => requests ?? (requests = new AppRequestRepository(this));
        internal AppRequest Request(AppRequestRecord record) =>
            new AppRequest(this, record);

        private AppEventRepository events;
        public AppEventRepository Events()
            => events ?? (events = new AppEventRepository(this));
        internal AppEvent Event(AppEventRecord record) => new AppEvent(record);

        public Task Transaction(Func<Task> action) => DB.Transaction(action);
    }
}
