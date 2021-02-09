using MainDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_Core;

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
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public AppKey Key() => new AppKey(record.Name, AppType.Values.Value(record.Type));
        public string Title { get => record.Title; }

        public async Task<ModifierCategory> TryAddModCategory(ModifierCategoryName name)
        {
            var modCategory = await factory.ModCategories().Category(this, name);
            if (modCategory.ID.IsNotValid() || !modCategory.Name().Equals(name))
            {
                modCategory = await factory.ModCategories().Add(this, name);
            }
            return modCategory;
        }

        public Task<IEnumerable<ModifierCategory>> ModCategories()
            => factory.ModCategories().Categories(this);

        public Task<ModifierCategory> ModCategory(int modCategoryID)
            => factory.ModCategories().Category(this, modCategoryID);

        public Task<ModifierCategory> ModCategory(ModifierCategoryName name)
            => factory.ModCategories().Category(this, name);

        public Task<AppRole> AddRole(AppRoleName name) =>
            factory.Roles().Add(this, name);

        async Task<IEnumerable<IAppRole>> IApp.Roles() =>
            await factory.Roles().RolesForApp(this);

        public Task<AppRole[]> Roles() => factory.Roles().RolesForApp(this);

        public Task<AppRole> Role(int roleID) =>
            factory.Roles().Role(this, roleID);

        public Task<AppRole> Role(AppRoleName roleName) =>
            factory.Roles().Role(this, roleName);

        public Task<AppUserRole> UserRole(int userRoleID) =>
            factory.UserRoles().UserRole(this, userRoleID);

        public Task<AppVersion> NewVersion(AppVersionKey versionKey, AppVersionType type, Version version, DateTimeOffset timeAdded)
            => factory.Versions().Create(versionKey, this, type, version, timeAdded);

        public Task<AppVersion> StartNewPatch(DateTimeOffset timeAdded) =>
            startNewVersion(timeAdded, AppVersionType.Values.Patch);

        public Task<AppVersion> StartNewMinorVersion(DateTimeOffset timeAdded) =>
            startNewVersion(timeAdded, AppVersionType.Values.Minor);

        public Task<AppVersion> StartNewMajorVersion(DateTimeOffset timeAdded) =>
            startNewVersion(timeAdded, AppVersionType.Values.Major);

        private Task<AppVersion> startNewVersion(DateTimeOffset timeAdded, AppVersionType type)
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

        public async Task<IEnumerable<AppRequestExpandedModel>> MostRecentRequests(int howMany)
        {
            var version = await CurrentVersion();
            var requests = await version.MostRecentRequests(howMany);
            return requests;
        }

        public async Task<IEnumerable<AppEvent>> MostRecentErrorEvents(int howMany)
        {
            var version = await CurrentVersion();
            var requests = await version.MostRecentErrorEvents(howMany);
            return requests;
        }

        public AppModel ToAppModel()
        {
            var key = Key();
            return new AppModel
            {
                ID = ID.Value,
                AppName = key.Name.DisplayText,
                Title = record.Title,
                Type = key.Type
            };
        }

        public override string ToString() => $"{nameof(App)} {ID.Value}: {record.Name}";

    }
}
