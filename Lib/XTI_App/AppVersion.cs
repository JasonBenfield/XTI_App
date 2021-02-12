using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppVersion : IAppVersion
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppVersionRecord> repo;
        private readonly AppVersionRecord record;

        internal AppVersion(AppFactory factory, DataRepository<AppVersionRecord> repo, AppVersionRecord record)
        {
            this.factory = factory;
            this.repo = repo;
            this.record = record ?? new AppVersionRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        private int Major { get => record.Major; }
        private int Minor { get => record.Minor; }
        private int Patch { get => record.Patch; }

        public AppVersionKey Key() => AppVersionKey.Parse(record.VersionKey);

        public bool IsPublishing() => Status().Equals(AppVersionStatus.Values.Publishing);
        public bool IsCurrent() => Status().Equals(AppVersionStatus.Values.Current);
        public bool IsNew() => Status().Equals(AppVersionStatus.Values.New);

        public bool IsPatch() => Type().Equals(AppVersionType.Values.Patch);
        public bool IsMinor() => Type().Equals(AppVersionType.Values.Minor);
        public bool IsMajor() => Type().Equals(AppVersionType.Values.Major);

        private AppVersionStatus Status() => AppVersionStatus.Values.Value(record.Status);
        public AppVersionType Type() => AppVersionType.Values.Value(record.Type);

        public Version Version() => new Version(Major, Minor, Patch);
        public Version NextMajor() => new Version(Major + 1, 0, 0);
        public Version NextMinor() => new Version(Major, Minor + 1, 0);
        public Version NextPatch() => new Version(Major, Minor, Patch + 1);

        public async Task<AppVersion> Current()
        {
            var app = await factory.Apps().App(record.AppID);
            var current = await app.CurrentVersion();
            return current;
        }

        public async Task Publishing()
        {
            var current = await Current();
            await repo.Update(record, r =>
            {
                r.Status = AppVersionStatus.Values.Publishing.Value;
                var type = Type();
                Version nextVersion;
                if (type.Equals(AppVersionType.Values.Major))
                {
                    nextVersion = current.NextMajor();
                }
                else if (type.Equals(AppVersionType.Values.Minor))
                {
                    nextVersion = current.NextMinor();
                }
                else if (type.Equals(AppVersionType.Values.Patch))
                {
                    nextVersion = current.NextPatch();
                }
                else
                {
                    throw new NotSupportedException($"Version type '{type}' is not supported");
                }
                r.Major = nextVersion.Major;
                r.Minor = nextVersion.Minor;
                r.Patch = nextVersion.Build;
            });
        }

        public async Task Published()
        {
            if (!IsPublishing())
            {
                throw new ArgumentException($"Cannot publish when status is '{Status().DisplayText}'");
            }
            var app = await factory.Apps().App(record.AppID);
            var current = await app.CurrentVersion();
            if (current.IsCurrent())
            {
                await current.Archive();
            }
            await repo.Update(record, r =>
            {
                r.Status = AppVersionStatus.Values.Current.Value;
            });
        }

        private Task Archive()
        {
            return repo.Update(record, r =>
            {
                r.Status = AppVersionStatus.Values.Old.Value;
            });
        }

        public Task<IEnumerable<AppRequestExpandedModel>> MostRecentRequests(int howMany)
            => factory.Requests().MostRecentForVersion(this, howMany);

        public Task<IEnumerable<AppEvent>> MostRecentErrorEvents(int howMany)
            => factory.Events().MostRecentErrorsForVersion(this, howMany);

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

        public Task<IEnumerable<ResourceGroup>> ResourceGroups()
            => factory.Groups().Groups(this);

        async Task<IResourceGroup> IAppVersion.ResourceGroup(ResourceGroupName name)
            => await ResourceGroup(name);

        public Task<ResourceGroup> ResourceGroup(int id)
            => factory.Groups().Group(this, id);

        public Task<ResourceGroup> ResourceGroup(ResourceGroupName name)
            => factory.Groups().Group(this, name);

        public Task<Resource> Resource(int id)
            => factory.Resources().Resource(this, id);

        public AppVersionModel ToModel() => new AppVersionModel
        {
            ID = ID.Value,
            VersionKey = Key().DisplayText,
            Major = Major,
            Minor = Minor,
            Patch = Patch,
            VersionType = Type(),
            Status = Status(),
            TimeAdded = record.TimeAdded
        };

        public override string ToString() => $"{nameof(AppVersion)} {ID.Value}: {Version()}";

    }
}
