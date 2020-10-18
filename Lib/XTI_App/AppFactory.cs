using System;
using XTI_App.Entities;
using XTI_Core;

namespace XTI_App
{
    public abstract class AppFactory
    {
        protected AppFactory() { }

        private AppUserRepository users;
        public AppUserRepository Users() =>
            fetchRepo<AppUserRecord, AppUserRepository>(ref users,
                dataRepo => new AppUserRepository(this, dataRepo));
        internal AppUser User(AppUserRecord record) => new AppUser(CreateDataRepository<AppUserRecord>(), this, record);

        private AppSessionRepository sessions;
        public AppSessionRepository Sessions() =>
            fetchRepo<AppSessionRecord, AppSessionRepository>(ref sessions,
                dataRepo => new AppSessionRepository(this, dataRepo));
        internal AppSession Session(AppSessionRecord record) =>
            new AppSession(this, CreateDataRepository<AppSessionRecord>(), record);

        private AppRequestRepository requests;
        public AppRequestRepository Requests() =>
            fetchRepo<AppRequestRecord, AppRequestRepository>(ref requests,
                dataRepo => new AppRequestRepository(this, dataRepo));
        internal AppRequest Request(AppRequestRecord record) =>
            new AppRequest(this, CreateDataRepository<AppRequestRecord>(), record);

        private AppEventRepository events;
        public AppEventRepository Events() =>
            fetchRepo<AppEventRecord, AppEventRepository>(ref events,
                dataRepo => new AppEventRepository(this, dataRepo));
        internal AppEvent Event(AppEventRecord record) => new AppEvent(record);

        private AppRepository apps;
        public AppRepository Apps() =>
            fetchRepo<AppRecord, AppRepository>(ref apps, dataRepo =>
                new AppRepository(this, dataRepo));
        internal App App(AppRecord record) =>
            new App(CreateDataRepository<AppRecord>(), this, record);

        private AppVersionRepository versionRepo;
        public AppVersionRepository Versions() =>
            fetchRepo<AppVersionRecord, AppVersionRepository>(ref versionRepo, dataRepo =>
                new AppVersionRepository(this, dataRepo));
        internal AppVersion Version(AppVersionRecord record) => new AppVersion(this, CreateDataRepository<AppVersionRecord>(), record);

        private AppRoleRepository roles;
        internal AppRoleRepository Roles() =>
            fetchRepo<AppRoleRecord, AppRoleRepository>(ref roles, dataRepo => new AppRoleRepository(this, dataRepo));
        internal AppRole Role(AppRoleRecord record) => new AppRole(CreateDataRepository<AppRoleRecord>(), record);

        private AppUserRoleRepository userRoles;
        internal AppUserRoleRepository UserRoles() =>
            fetchRepo<AppUserRoleRecord, AppUserRoleRepository>
            (
                ref userRoles, dataRepo => new AppUserRoleRepository(this, dataRepo)
            );
        internal AppUserRole UserRole(AppUserRoleRecord record) =>
            new AppUserRole(CreateDataRepository<AppUserRoleRecord>(), record);

        private TRepo fetchRepo<TRecord, TRepo>
        (
            ref TRepo repo,
            Func<DataRepository<TRecord>, TRepo> createRepo
        )
            where TRecord : class
        {
            return repo ?? (repo = createRepo(CreateDataRepository<TRecord>()));
        }

        protected abstract DataRepository<T> CreateDataRepository<T>() where T : class;

    }
}
