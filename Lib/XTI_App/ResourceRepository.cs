using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class ResourceRepository
    {
        private readonly AppFactory factory;

        internal ResourceRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        public async Task<Resource> Add(ResourceGroup group, ResourceName name, ResourceResultType resultType)
        {
            var record = new ResourceRecord
            {
                GroupID = group.ID.Value,
                Name = name.Value,
                ResultType = resultType.Value
            };
            await factory.DB.Resources.Create(record);
            return factory.Resource(record);
        }

        public Task<Resource[]> Resources(ResourceGroup group)
            => factory.DB
                .Resources
                .Retrieve()
                .Where(r => r.GroupID == group.ID.Value)
                .OrderBy(r => r.ResultType)
                .ThenBy(r => r.Name)
                .Select(r => factory.Resource(r))
                .ToArrayAsync();

        public async Task<Resource> Resource(ResourceGroup group, ResourceName name)
        {
            var record = await factory.DB
                .Resources
                .Retrieve()
                .FirstOrDefaultAsync(r => r.GroupID == group.ID.Value && r.Name == name.Value);
            if (record == null)
            {
                record = await factory.DB
                    .Resources
                    .Retrieve()
                   .FirstOrDefaultAsync(r => r.GroupID == group.ID.Value && r.Name == ResourceName.Unknown.Value);
                if (record == null)
                {
                    record = await factory.DB
                        .Resources
                        .Retrieve()
                       .FirstOrDefaultAsync(r => r.Name == ResourceName.Unknown.Value);
                }
            }
            return factory.Resource(record);
        }

        internal async Task<Resource> Resource(AppVersion version, int id)
        {
            var groupIDs = factory.DB
                .ResourceGroups
                .Retrieve()
                .Where(rg => rg.VersionID == version.ID.Value)
                .Select(rg => rg.ID);
            var record = await factory.DB
                .Resources
                .Retrieve()
                .FirstOrDefaultAsync(r => r.ID == id && groupIDs.Any(gID => gID == r.GroupID));
            return factory.Resource(record);
        }
    }
}
