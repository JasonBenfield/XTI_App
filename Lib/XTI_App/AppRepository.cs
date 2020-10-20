using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using XTI_App.Entities;
using XTI_Core;

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

        public async Task<App> AddApp(AppKey key, AppType type, string title, DateTime timeAdded)
        {
            var record = new AppRecord
            {
                Key = key.Value,
                Type = type.Value,
                Title = title?.Trim() ?? "",
                TimeAdded = timeAdded
            };
            await repo.Create(record);
            return factory.App(record);
        }

        public async Task<App> App(int id)
        {
            var record = await repo.Retrieve().FirstOrDefaultAsync(a => a.ID == id);
            return factory.App(record);
        }

        public Task<App> WebApp(AppKey key) => App(key, AppType.Values.WebApp);

        public async Task<App> App(AppKey key, AppType type)
        {
            var record = await repo.Retrieve()
                .FirstOrDefaultAsync(a => a.Key == key.Value && a.Type == type.Value);
            if (record == null)
            {
                record = await repo.Retrieve()
                    .FirstOrDefaultAsync(a => a.Key == AppKey.Unknown.Value);
            }
            return factory.App(record);
        }
    }
}
