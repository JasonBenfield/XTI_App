using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class ModifierCategoryRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<ModifierCategoryRecord> repo;

        internal ModifierCategoryRepository(AppFactory factory, DataRepository<ModifierCategoryRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        internal async Task<ModifierCategory> Add(IApp app, ModifierCategoryName name)
        {
            var record = new ModifierCategoryRecord
            {
                AppID = app.ID.Value,
                Name = name.Value
            };
            await repo.Create(record);
            return factory.ModCategory(record);
        }

        internal async Task<ModifierCategory> Category(int id)
        {
            var record = await repo.Retrieve()
                .FirstOrDefaultAsync(c => c.ID == id);
            return factory.ModCategory(record);
        }

        internal async Task<IEnumerable<ModifierCategory>> Categories(IApp app)
        {
            var records = await repo.Retrieve()
                .Where(c => c.AppID == app.ID.Value)
                .ToArrayAsync();
            return records.Select(c => factory.ModCategory(c));
        }

        internal async Task<ModifierCategory> Category(App app, int id)
        {
            var record = await repo.Retrieve()
                .FirstOrDefaultAsync(c => c.AppID == app.ID.Value && c.ID == id);
            return factory.ModCategory(record);
        }

        internal async Task<ModifierCategory> Category(IApp app, ModifierCategoryName name)
        {
            var record = await repo.Retrieve()
                .FirstOrDefaultAsync(c => c.AppID == app.ID.Value && c.Name == name.Value);
            if (record == null)
            {
                record = await repo.Retrieve()
                    .FirstOrDefaultAsync(c => c.AppID == app.ID.Value && c.Name == ModifierCategoryName.Default.Value);
            }
            return factory.ModCategory(record);
        }
    }
}
