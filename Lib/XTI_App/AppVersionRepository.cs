using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class AppVersionRepository
    {
        private readonly AppFactory factory;

        internal AppVersionRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        internal async Task<AppVersion> Create(AppVersionKey key, App app, AppVersionType type, Version version, DateTimeOffset timeAdded)
        {
            var record = new AppVersionRecord
            {
                VersionKey = key.Value,
                AppID = app.ID.Value,
                Major = version.Major,
                Minor = version.Minor,
                Patch = version.Build,
                TimeAdded = timeAdded,
                Description = "",
                Status = AppVersionStatus.Values.New.Value,
                Type = type.Value
            };
            await factory.DB.Versions.Create(record);
            return factory.Version(record);
        }

        internal async Task<AppVersion> StartNewVersion(AppVersionKey key, App app, DateTimeOffset timeAdded, AppVersionType type)
        {
            AppVersionRecord record = null;
            await factory.DB.Transaction(async () =>
            {
                record = new AppVersionRecord
                {
                    VersionKey = new GeneratedKey().Value(),
                    AppID = app.ID.Value,
                    Major = 0,
                    Minor = 0,
                    Patch = 0,
                    TimeAdded = timeAdded,
                    Description = "",
                    Status = AppVersionStatus.Values.New.Value,
                    Type = type.Value
                };
                await factory.DB.Versions.Create(record);
                if (key.Equals(AppVersionKey.None))
                {
                    await factory.DB.Versions.Update(record, r => r.VersionKey = new AppVersionKey(r.ID).Value);
                }
            });
            return factory.Version(record);
        }

        public async Task<AppVersion> Version(int id)
        {
            var record = await factory.DB
                .Versions
                .Retrieve()
                .FirstOrDefaultAsync(v => v.ID == id);
            return factory.Version(record);
        }

        public async Task<AppVersion> Version(AppVersionKey versionKey)
        {
            if (versionKey.Equals(AppVersionKey.Current))
            {
                throw new ArgumentException("App is required when version key is current");
            }
            var record = await factory.DB
                .Versions
                .Retrieve()
                .FirstOrDefaultAsync(v => v.VersionKey == versionKey.Value);
            return factory.Version(record);
        }

        internal Task<AppVersion[]> VersionsByApp(App app)
            => factory.DB
                .Versions
                .Retrieve()
                .Where(v => v.AppID == app.ID.Value)
                .Select(v => factory.Version(v))
                .ToArrayAsync();

        internal async Task<AppVersion> CurrentVersion(App app)
        {
            var record = await factory.DB
                .Versions
                .Retrieve()
                .Where(v => v.AppID == app.ID.Value && v.Status == AppVersionStatus.Values.Current.Value)
                .FirstOrDefaultAsync();
            return factory.Version(record);
        }
    }
}
