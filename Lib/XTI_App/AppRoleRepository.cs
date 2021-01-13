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

        internal async Task<IEnumerable<AppRole>> RolesForApp(App app)
        {
            var records = await repoFactory.CreateRoles().Retrieve()
                .Where(r => r.AppID == app.ID.Value)
                .ToArrayAsync();
            return records.Select(r => factory.Role(r));
        }

        internal async Task<AppRole> Role(App app, AppRoleName roleName)
        {
            var record = await repoFactory.CreateRoles().Retrieve()
                .Where(r => r.AppID == app.ID.Value && r.Name == roleName.Value)
                .FirstOrDefaultAsync();
            return factory.Role(record);
        }

        internal Task<IEnumerable<AppRole>> AllowedRolesForResourceGroup(ResourceGroup group)
            => rolesForResourceGroup(group, true);

        internal Task<IEnumerable<AppRole>> DeniedRolesForResourceGroup(ResourceGroup group)
            => rolesForResourceGroup(group, false);

        private async Task<IEnumerable<AppRole>> rolesForResourceGroup(ResourceGroup group, bool isAllowed)
        {
            var roleIDs = repoFactory.CreateResourceGroupRoles()
                .Retrieve()
                .Where(gr => gr.GroupID == group.ID.Value && gr.IsAllowed == isAllowed)
                .Select(gr => gr.RoleID);
            var records = await repoFactory.CreateRoles()
                .Retrieve()
                .Where(r => roleIDs.Any(id => id == r.ID))
                .ToArrayAsync();
            return records.Select(r => factory.Role(r));
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
