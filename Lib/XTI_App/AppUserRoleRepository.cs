using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppUserRoleRepository
    {
        private readonly IMainDataRepositoryFactory repoFactory;
        private readonly AppFactory factory;
        private readonly DataRepository<AppUserRoleRecord> repo;

        internal AppUserRoleRepository(IMainDataRepositoryFactory repoFactory, AppFactory factory)
        {
            this.repoFactory = repoFactory;
            this.factory = factory;
            repo = repoFactory.CreateUserRoles();
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
            var roleIDs = repoFactory.CreateRoles()
                .Retrieve()
                .Where(r => r.AppID == app.ID.Value)
                .Select(r => r.ID);
            var records = await repo.Retrieve()
                .Where
                (
                    ur =>
                        ur.UserID == user.ID.Value
                        && roleIDs.Any(id => id == ur.RoleID)
                )
                .ToArrayAsync();
            return records.Select(ur => factory.UserRole(ur));
        }
    }
}
