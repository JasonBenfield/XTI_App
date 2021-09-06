using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class ResourceGroupRepository
    {
        private readonly AppFactory factory;

        internal ResourceGroupRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        internal async Task<ResourceGroup> Add(AppVersion version, ResourceGroupName name, ModifierCategory modCategory)
        {
            var record = new ResourceGroupRecord
            {
                VersionID = version.ID.Value,
                Name = name.Value,
                ModCategoryID = modCategory.ID.Value
            };
            await factory.DB.ResourceGroups.Create(record);
            return factory.Group(record);
        }

        internal Task<ResourceGroup[]> Groups(AppVersion version)
            => factory.DB
                .ResourceGroups
                .Retrieve()
                .Where(g => g.VersionID == version.ID.Value)
                .OrderBy(g => g.Name)
                .Select(g => factory.Group(g))
                .ToArrayAsync();

        internal async Task<ResourceGroup> Group(AppVersion version, ResourceGroupName name)
        {
            var record = await factory.DB
                .ResourceGroups
                .Retrieve()
                .Where(g => g.VersionID == version.ID.Value && g.Name == name.Value)
                .FirstOrDefaultAsync();
            if (record == null)
            {
                record = await factory.DB
                    .ResourceGroups
                    .Retrieve()
                    .Where(g => g.VersionID == version.ID.Value && g.Name == ResourceGroupName.Unknown.Value)
                    .FirstOrDefaultAsync();
                if (record == null)
                {
                    record = await factory.DB
                        .ResourceGroups
                        .Retrieve()
                        .Where(g => g.Name == ResourceGroupName.Unknown.Value)
                        .FirstOrDefaultAsync();
                }
            }
            return factory.Group(record);
        }

        internal async Task<ResourceGroup> GroupForVersion(AppVersion version, int id)
        {
            var record = await factory.DB
                .ResourceGroups
                .Retrieve()
                .Where(g => g.VersionID == version.ID.Value && g.ID == id)
                .FirstOrDefaultAsync();
            return factory.Group(record);
        }

        public async Task<ResourceGroup> Group(int id)
        {
            var record = await factory.DB
                .ResourceGroups
                .Retrieve()
                .Where(g => g.ID == id)
                .FirstOrDefaultAsync();
            return factory.Group(record);
        }

        internal Task<ResourceGroup[]> Groups(ModifierCategory modCategory)
            => factory.DB
                .ResourceGroups
                .Retrieve()
                .Where(g => g.ModCategoryID == modCategory.ID.Value)
                .OrderBy(g => g.Name)
                .Select(g => factory.Group(g))
                .ToArrayAsync();
    }
}
