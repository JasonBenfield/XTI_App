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

        public Task<ModifierCategory> ModCategory(ModifierCategoryName name)
            => factory.ModCategories().Category(this, name);

        public async Task<ResourceGroup> AddOrUpdateResourceGroup(ResourceGroupName name, ModifierCategory modCategory)
        {
            var resourceGroup = await ResourceGroup(name);
            if (resourceGroup.Name().Equals(name))
            {
                await resourceGroup.SetModCategory(modCategory);
            }
            else
            {
                resourceGroup = await AddResourceGroup(name, modCategory);
            }
            return resourceGroup;
        }

        public Task<ResourceGroup> AddResourceGroup(ResourceGroupName name, ModifierCategory modCategory)
            => factory.Groups().Add(this, name, modCategory);

        public Task<ResourceGroup> ResourceGroup(ResourceGroupName name) => factory.Groups().Group(this, name);

        public Task<IEnumerable<ResourceGroup>> ResourceGroups() => factory.Groups().Groups(this);

        async Task<IResourceGroup> IApp.ResourceGroup(ResourceGroupName name) => await ResourceGroup(name);

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
