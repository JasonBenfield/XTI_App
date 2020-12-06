using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppRoleRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppRoleRecord> repo;

        internal AppRoleRepository(AppFactory factory, DataRepository<AppRoleRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        internal async Task<AppRole> Add(App app, AppRoleName name)
        {
            var record = new AppRoleRecord
            {
                AppID = app.ID.Value,
                Name = name.Value
            };
            await repo.Create(record);
            return factory.Role(record);
        }

        internal async Task<IEnumerable<AppRole>> RolesForApp(App app)
        {
            var records = await repo.Retrieve()
                .Where(r => r.AppID == app.ID.Value)
                .ToArrayAsync();
            return records.Select(r => factory.Role(r));
        }

        internal async Task<AppRole> Role(App app, AppRoleName roleName)
        {
            var record = await repo.Retrieve()
                .Where(r => r.AppID == app.ID.Value && r.Name == roleName.Value)
                .FirstOrDefaultAsync();
            return factory.Role(record);
        }

        internal IQueryable<int> RoleIDsForApp(IApp app)
        {
            return repo.Retrieve()
                .Where(r => r.AppID == app.ID.Value)
                .Select(r => r.ID);
        }
    }
}
