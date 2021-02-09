using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        internal Task<AppUserRoleModel[]> AssignedRoles(AppUser user, App app)
        {
            return repo
                .Retrieve()
                .Where(ur => ur.UserID == user.ID.Value)
                .Join
                (
                    repoFactory.CreateRoles()
                        .Retrieve()
                        .Where(r => r.AppID == app.ID.Value),
                    ur => ur.RoleID,
                    r => r.ID,
                    (ur, r) => new AppUserRoleModel
                    {
                        ID = ur.ID,
                        Role = factory.Role(r).ToModel()
                    }
                )
                .ToArrayAsync();
        }

        internal async Task<AppUserRole> UserRole(App app, int userRoleID)
        {
            var roleIDs = repoFactory.CreateRoles()
                .Retrieve()
                .Where(r => r.AppID == app.ID.Value)
                .Select(r => r.ID);
            var record = await repo
                .Retrieve()
                .Where(ur => ur.ID == userRoleID && roleIDs.Any(roleID => roleID == ur.RoleID))
                .FirstOrDefaultAsync();
            return factory.UserRole(record);
        }
    }
}
