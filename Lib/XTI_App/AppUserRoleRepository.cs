using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppUserRoleRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppUserRoleRecord> repo;

        internal AppUserRoleRepository(AppFactory factory, DataRepository<AppUserRoleRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        internal async Task<AppUserRole> Add(AppUser user, AppRole role)
        {
            var record = new AppUserRoleRecord
            {
                UserID = user.ID.Value,
                RoleID = role.ID.Value
            };
            await repo.Create(record);
            return factory.UserRole(record);
        }

        internal async Task<IEnumerable<AppUserRole>> RolesForUser(IAppUser user, IApp app)
        {
            var roleRepo = factory.Roles();
            var records = await repo.Retrieve()
                .Where
                (
                    ur =>
                        ur.UserID == user.ID.Value
                        && roleRepo.RoleIDsForApp(app).Any(id => id == ur.RoleID)
                )
                .ToArrayAsync();
            return records.Select(ur => factory.UserRole(ur));
        }
    }
}
