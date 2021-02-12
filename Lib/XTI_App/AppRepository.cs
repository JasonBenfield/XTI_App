using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;
using System.Collections.Generic;
using System.Linq;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class AppRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppRecord> repo;

        internal AppRepository(AppFactory factory, DataRepository<AppRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        public async Task<App> AddOrUpdate(AppKey appKey, string title, DateTimeOffset timeAdded)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = appKey.Name.DisplayText;
            }
            var app = await App(appKey);
            if (app.ID.IsValid() && app.Key().Equals(appKey))
            {
                await app.SetTitle(title);
            }
            else
            {
                app = await Add(appKey, title, timeAdded);
            }
            return app;
        }

        public async Task<App> Add(AppKey appKey, string title, DateTimeOffset timeAdded)
        {
            var record = new AppRecord
            {
                Name = appKey.Name.Value,
                Type = appKey.Type.Value,
                Title = title?.Trim() ?? "",
                TimeAdded = timeAdded
            };
            await repo.Create(record);
            return factory.App(record);
        }

        public async Task<IEnumerable<App>> All()
        {
            var records = await repo.Retrieve().ToArrayAsync();
            return records.Select(r => factory.App(r));
        }

        public async Task<App> App(int id)
        {
            var record = await repo.Retrieve().FirstOrDefaultAsync(a => a.ID == id);
            return factory.App(record);
        }

        public async Task<App> App(AppKey appKey)
        {
            var record = await repo.Retrieve()
                .FirstOrDefaultAsync(a => a.Name == appKey.Name.Value && a.Type == appKey.Type.Value);
            if (record == null)
            {
                record = await repo.Retrieve()
                    .FirstOrDefaultAsync(a => a.Name == AppName.Unknown.Value);
            }
            return factory.App(record);
        }
    }
}
