using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App
{
    public sealed class ResourceGroupRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<ResourceGroupRecord> repo;

        internal ResourceGroupRepository(AppFactory factory, DataRepository<ResourceGroupRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        internal async Task<ResourceGroup> Add(AppVersion version, ResourceGroupName name, ModifierCategory modCategory)
        {
            var record = new ResourceGroupRecord
            {
                VersionID = version.ID.Value,
                Name = name.Value,
                ModCategoryID = modCategory.ID.Value
            };
            await repo.Create(record);
            return factory.Group(record);
        }

        internal async Task<IEnumerable<ResourceGroup>> Groups(AppVersion version)
        {
            var records = await repo.Retrieve()
                .Where(g => g.VersionID == version.ID.Value)
                .OrderBy(g => g.Name)
                .ToArrayAsync();
            return records.Select(g => factory.Group(g));
        }

        internal async Task<ResourceGroup> Group(AppVersion version, ResourceGroupName name)
        {
            var record = await repo.Retrieve()
                .Where(g => g.VersionID == version.ID.Value && g.Name == name.Value)
                .FirstOrDefaultAsync();
            if (record == null)
            {
                record = await repo.Retrieve()
                    .Where(g => g.VersionID == version.ID.Value && g.Name == ResourceGroupName.Unknown.Value)
                    .FirstOrDefaultAsync();
                if (record == null)
                {
                    record = await repo.Retrieve()
                        .Where(g => g.Name == ResourceGroupName.Unknown.Value)
                        .FirstOrDefaultAsync();
                }
            }
            return factory.Group(record);
        }

        internal async Task<ResourceGroup> Group(AppVersion version, int id)
        {
            var record = await repo.Retrieve()
                .Where(g => g.VersionID == version.ID.Value && g.ID == id)
                .FirstOrDefaultAsync();
            return factory.Group(record);
        }

        internal async Task<IEnumerable<ResourceGroup>> Groups(ModifierCategory modCategory)
        {
            var records = await repo.Retrieve()
                .Where(g => g.ModCategoryID == modCategory.ID.Value)
                .OrderBy(g => g.Name)
                .ToArrayAsync();
            return records.Select(g => factory.Group(g));
        }
    }
}
