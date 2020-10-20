using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppVersionRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppVersionRecord> repo;

        internal AppVersionRepository(AppFactory factory, DataRepository<AppVersionRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        internal async Task<AppVersion> StartNewVersion(AppVersionKey key, App app, DateTime timeAdded, AppVersionType type)
        {
            AppVersionRecord record = null;
            await repo.Transaction(async () =>
            {
                record = new AppVersionRecord
                {
                    VersionKey = key.Value,
                    AppID = app.ID.Value,
                    Major = 0,
                    Minor = 0,
                    Patch = 0,
                    TimeAdded = timeAdded,
                    Description = "",
                    Status = AppVersionStatus.Values.New.Value,
                    Type = type.Value
                };
                await repo.Create(record);
                if (key.Equals(AppVersionKey.None))
                {
                    await repo.Update(record, r => r.VersionKey = new AppVersionKey(r.ID).Value);
                }
            });
            return factory.Version(record);
        }

        public async Task<AppVersion> Version(int id)
        {
            var record = await repo.Retrieve()
                .FirstOrDefaultAsync(v => v.ID == id);
            return factory.Version(record);
        }

        public async Task<AppVersion> Version(AppVersionKey versionKey)
        {
            if (versionKey.Equals(AppVersionKey.Current))
            {
                throw new ArgumentException("App is required when version key is current");
            }
            var record = await repo.Retrieve()
                .FirstOrDefaultAsync(v => v.VersionKey == versionKey.Value);
            return factory.Version(record);
        }

        internal async Task<IEnumerable<AppVersion>> VersionsByApp(App app)
        {
            var records = await repo.Retrieve().Where(v => v.AppID == app.ID.Value).ToArrayAsync();
            return records.Select(v => factory.Version(v));
        }

        internal async Task<AppVersion> CurrentVersion(App app)
        {
            var record = await repo.Retrieve()
                .Where(v => v.AppID == app.ID.Value && v.Status == AppVersionStatus.Values.Current.Value)
                .FirstOrDefaultAsync();
            return factory.Version(record);
        }
    }
}
