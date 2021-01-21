using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class ResourceRepository
    {
        private readonly IMainDataRepositoryFactory repoFactory;
        private readonly AppFactory factory;
        private readonly DataRepository<ResourceRecord> repo;

        internal ResourceRepository(IMainDataRepositoryFactory repoFactory, AppFactory factory)
        {
            this.factory = factory;
            this.repoFactory = repoFactory;
            repo = repoFactory.CreateResources();
        }

        public async Task<Resource> Add(ResourceGroup group, ResourceName name, ResourceResultType resultType)
        {
            var record = new ResourceRecord
            {
                GroupID = group.ID.Value,
                Name = name.Value,
                ResultType = resultType.Value
            };
            await repo.Create(record);
            return factory.Resource(record);
        }

        public async Task<IEnumerable<Resource>> Resources(ResourceGroup group)
        {
            var records = await repo.Retrieve()
                .Where(r => r.GroupID == group.ID.Value)
                .OrderBy(r => r.ResultType)
                .ThenBy(r => r.Name)
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

        internal async Task<Resource> Resource(App app, int id)
        {
            var groupIDs = repoFactory.CreateResourceGroups()
                .Retrieve()
                .Where(rg => rg.AppID == app.ID.Value)
                .Select(rg => rg.ID);
            var record = await repo.Retrieve()
                .FirstOrDefaultAsync(r => r.ID == id && groupIDs.Any(gID => gID == r.GroupID));
            return factory.Resource(record);
        }
    }
}
