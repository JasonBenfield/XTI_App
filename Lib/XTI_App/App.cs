using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTI_App
{
    public sealed class App : IApp
    {
        private readonly DataRepository<AppRecord> repo;
        private readonly AppFactory factory;
        private readonly AppRecord record;

        internal App(DataRepository<AppRecord> repo, AppFactory factory, AppRecord record)
        {
            this.repo = repo;
            this.factory = factory;
            this.record = record ?? new AppRecord();
        }

        public int ID { get => record.ID; }
        public AppKey Key() => new AppKey(record.Key);
        public string Title { get => record.Title; }
        public bool Exists() => ID > 0;

        public Task<AppRole> AddRole(AppRoleName name) =>
            factory.Roles().Add(this, name);

        async Task<IEnumerable<IAppRole>> IApp.Roles() =>
            await factory.Roles().RolesForApp(this);

        public Task<IEnumerable<AppRole>> Roles() =>
            factory.Roles().RolesForApp(this);

        public Task<AppRole> Role(AppRoleName roleName) =>
            factory.Roles().Role(this, roleName);

        public Task<AppVersion> StartNewPatch(DateTime timeAdded) =>
            startNewVersion(timeAdded, AppVersionType.Values.Patch);

        public Task<AppVersion> StartNewMinorVersion(DateTime timeAdded) =>
            startNewVersion(timeAdded, AppVersionType.Values.Minor);

        public Task<AppVersion> StartNewMajorVersion(DateTime timeAdded) =>
            startNewVersion(timeAdded, AppVersionType.Values.Major);

        private Task<AppVersion> startNewVersion(DateTime timeAdded, AppVersionType type)
        {
            return factory.Versions().StartNewVersion(AppVersionKey.None, this, timeAdded, type);
        }

        public Task<AppVersion> CurrentVersion() =>
            factory.Versions().CurrentVersion(this);

        public async Task SetRoles(IEnumerable<AppRoleName> roleNames)
        {
            var existingRoles = (await Roles()).ToArray();
            await repo.Transaction(async () =>
            {
                await addRoles(roleNames, existingRoles);
                var rolesToDelete = existingRoles
                    .Where(r => !roleNames.Any(rn => r.Name().Equals(rn)))
                    .ToArray();
                await deleteRoles(rolesToDelete);
            });
        }

        private async Task addRoles(IEnumerable<AppRoleName> roleNames, IEnumerable<AppRole> existingRoles)
        {
            foreach (var roleName in roleNames)
            {
                if (!existingRoles.Any(r => r.Name().Equals(roleName)))
                {
                    await AddRole(roleName);
                }
            }
        }

        private static async Task deleteRoles(IEnumerable<AppRole> rolesToDelete)
        {
            foreach (var role in rolesToDelete)
            {
                await role.Delete();
            }
        }

        async Task<IAppVersion> IApp.CurrentVersion() =>
            await factory.Versions().CurrentVersion(this);

        public async Task<AppVersion> Version(AppVersionKey versionKey) =>
            (AppVersion)await version(versionKey);

        Task<IAppVersion> IApp.Version(AppVersionKey versionKey) => version(versionKey);

        private async Task<IAppVersion> version(AppVersionKey versionKey)
        {
            AppVersion version;
            if (versionKey.Equals(AppVersionKey.Current))
            {
                version = await CurrentVersion();
            }
            else
            {
                var versions = await Versions();
                version = versions.First(v => v.Key().Equals(versionKey));
            }
            return version;
        }

        public Task<IEnumerable<AppVersion>> Versions() =>
            factory.Versions().VersionsByApp(this);

        public Task SetTitle(string title)
        {
            return repo.Update(record, r =>
            {
                r.Title = title?.Trim() ?? "";
            });
        }

        public override string ToString() => $"{nameof(App)} {ID}: {record.Key}";
    }
}
