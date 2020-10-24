using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class ResourceRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<ResourceRecord> repo;

        internal ResourceRepository(AppFactory factory, DataRepository<ResourceRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        public async Task<Resource> Add(ResourceGroup group, ResourceName name)
        {
            var record = new ResourceRecord
            {
                GroupID = group.ID.Value,
                Name = name.Value
            };
            await repo.Create(record);
            return factory.Resource(record);
        }

        public async Task<IEnumerable<Resource>> Resources(ResourceGroup group)
        {
            var records = await repo.Retrieve()
                .Where(r => r.GroupID == group.ID.Value)
                .ToArrayAsync();
            return records.Select(r => factory.Resource(r));
        }

        public async Task<Resource> Resource(ResourceGroup group, ResourceName name)
        {
            var record = await repo.Retrieve()
                .FirstOrDefaultAsync(r => r.GroupID == group.ID.Value && r.Name == name.Value);
            if (record == null)
            {
                record = await repo.Retrieve()
                   .FirstOrDefaultAsync(r => r.Name == ResourceName.Unknown.Value);
            }
            return factory.Resource(record);
        }
    }
}
