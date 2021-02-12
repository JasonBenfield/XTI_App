using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class AppRoleRepository
    {
        private readonly AppFactory factory;
        private readonly IMainDataRepositoryFactory repoFactory;

        internal AppRoleRepository(IMainDataRepositoryFactory repoFactory, AppFactory factory)
        {
            this.factory = factory;
            this.repoFactory = repoFactory;
        }

        internal async Task<AppRole> Add(App app, AppRoleName name)
        {
            var record = new AppRoleRecord
            {
                AppID = app.ID.Value,
                Name = name.Value
            };
            await repoFactory.CreateRoles().Create(record);
            return factory.Role(record);
        }

        internal Task<AppRole[]> RolesForUser(IAppUser user, IApp app)
        {
            var roleIDs = repoFactory.CreateUserRoles()
                .Retrieve()
                .Where(ur => ur.UserID == user.ID.Value)
                .Select(ur => ur.RoleID);
            return repoFactory.CreateRoles()
                .Retrieve()
                .Where
                (
                     r => roleIDs.Any(id => r.ID == id) && r.AppID == app.ID.Value
                )
                .Select(r => factory.Role(r))
                .ToArrayAsync();
        }

        internal Task<AppRole[]> RolesForApp(App app)
        {
            return repoFactory.CreateRoles().Retrieve()
                .Where(r => r.AppID == app.ID.Value)
                .Select(r => factory.Role(r))
                .ToArrayAsync();
        }

        internal async Task<AppRole> Role(App app, int roleID)
        {
            var record = await rolesForApp(app)
                .Where(r => r.ID == roleID)
                .FirstOrDefaultAsync();
            return factory.Role(record);
        }

        internal async Task<AppRole> Role(App app, AppRoleName roleName)
        {
            var record = await rolesForApp(app)
                .Where(r => r.Name == roleName.Value)
                .FirstOrDefaultAsync();
            return factory.Role(record);
        }

        private IQueryable<AppRoleRecord> rolesForApp(App app)
        {
            return repoFactory.CreateRoles()
                .Retrieve()
                .Where(r => r.AppID == app.ID.Value);
        }

        internal Task<AppRole[]> AllowedRolesForResourceGroup(ResourceGroup group)
            => rolesForResourceGroup(group, true);

        internal Task<AppRole[]> DeniedRolesForResourceGroup(ResourceGroup group)
            => rolesForResourceGroup(group, false);

        private Task<AppRole[]> rolesForResourceGroup(ResourceGroup group, bool isAllowed)
        {
            var roleIDs = repoFactory.CreateResourceGroupRoles()
                .Retrieve()
                .Where(gr => gr.GroupID == group.ID.Value && gr.IsAllowed == isAllowed)
                .Select(gr => gr.RoleID);
            return repoFactory.CreateRoles()
                .Retrieve()
                .Where(r => roleIDs.Any(id => id == r.ID))
                .Select(r => factory.Role(r))
                .ToArrayAsync();
        }

        internal Task<AppRole[]> RolesNotAssignedToUser(AppUser user, App app)
        {
            var roleIDs = repoFactory.CreateUserRoles()
                .Retrieve()
                .Where(ur => ur.UserID == user.ID.Value)
                .Select(ur => ur.RoleID);
            return repoFactory.CreateRoles()
                .Retrieve()
                .Where(r => r.AppID == app.ID.Value && !roleIDs.Any(id => id == r.ID))
                .Select(r => factory.Role(r))
                .ToArrayAsync();
        }

        internal Task<IEnumerable<AppRole>> AllowedRolesForResource(Resource resource)
            => rolesForResource(resource, true);

        internal Task<IEnumerable<AppRole>> DeniedRolesForResource(Resource resource)
            => rolesForResource(resource, false);

        private async Task<IEnumerable<AppRole>> rolesForResource(Resource resource, bool isAllowed)
        {
            var roleIDs = repoFactory.CreateResourceRoles()
                .Retrieve()
                .Where(gr => gr.ResourceID == resource.ID.Value && gr.IsAllowed == isAllowed)
                .Select(gr => gr.RoleID);
            var records = await repoFactory.CreateRoles()
                .Retrieve()
                .Where(r => roleIDs.Any(id => id == r.ID))
                .ToArrayAsync();
            return records.Select(r => factory.Role(r));
        }

    }
}
