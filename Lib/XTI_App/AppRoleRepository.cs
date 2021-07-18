using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class AppRoleRepository
    {
        private readonly AppFactory factory;

        internal AppRoleRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        internal async Task<AppRole> Add(App app, AppRoleName name)
        {
            var record = new AppRoleRecord
            {
                AppID = app.ID.Value,
                Name = name.Value
            };
            await factory.DB.Roles.Create(record);
            return factory.Role(record);
        }

        internal Task<AppRole[]> RolesForApp(App app)
        {
            return factory.DB.Roles.Retrieve()
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
            return factory.DB.Roles
                .Retrieve()
                .Where(r => r.AppID == app.ID.Value);
        }

        internal Task<AppRole[]> AllowedRolesForResourceGroup(ResourceGroup group)
            => rolesForResourceGroup(group, true);

        internal Task<AppRole[]> DeniedRolesForResourceGroup(ResourceGroup group)
            => rolesForResourceGroup(group, false);

        private Task<AppRole[]> rolesForResourceGroup(ResourceGroup group, bool isAllowed)
        {
            var roleIDs = factory.DB
                .ResourceGroupRoles
                .Retrieve()
                .Where(gr => gr.GroupID == group.ID.Value && gr.IsAllowed == isAllowed)
                .Select(gr => gr.RoleID);
            return factory.DB
                .Roles
                .Retrieve()
                .Where(r => roleIDs.Any(id => id == r.ID))
                .Select(r => factory.Role(r))
                .ToArrayAsync();
        }

        internal Task<AppRole[]> RolesNotAssignedToUser(IAppUser user, IApp app, IModifier modifier)
        {
            var roleIDs = userRoleIDs(user, modifier);
            return factory.DB
                .Roles
                .Retrieve()
                .Where(r => r.AppID == app.ID.Value && !roleIDs.Any(id => id == r.ID))
                .Select(r => factory.Role(r))
                .ToArrayAsync();
        }

        internal Task<AppRole[]> RolesAssignedToUser(IAppUser user, IApp app, IModifier modifier)
        {
            var roleIDs = userRoleIDs(user, modifier);
            return factory.DB
                .Roles
                .Retrieve()
                .Where(r => r.AppID == app.ID.Value && roleIDs.Any(id => id == r.ID))
                .Select(r => factory.Role(r))
                .ToArrayAsync();
        }

        private IQueryable<int> userRoleIDs(IAppUser user, IModifier modifier)
        {
            return factory.DB
                .UserRoles
                .Retrieve()
                .Where(ur => ur.UserID == user.ID.Value && ur.ModifierID == modifier.ID.Value)
                .Select(ur => ur.RoleID);
        }

        internal Task<AppRole[]> AllowedRolesForResource(Resource resource)
            => rolesForResource(resource, true);

        internal Task<AppRole[]> DeniedRolesForResource(Resource resource)
            => rolesForResource(resource, false);

        private Task<AppRole[]> rolesForResource(Resource resource, bool isAllowed)
        {
            var roleIDs = factory.DB
                .ResourceRoles
                .Retrieve()
                .Where(gr => gr.ResourceID == resource.ID.Value && gr.IsAllowed == isAllowed)
                .Select(gr => gr.RoleID);
            return factory.DB
                .Roles
                .Retrieve()
                .Where(r => roleIDs.Any(id => id == r.ID))
                .Select(r => factory.Role(r))
                .ToArrayAsync();
        }

    }
}
