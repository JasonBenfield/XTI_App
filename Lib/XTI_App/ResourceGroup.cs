using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTI_App
{
    public sealed class ResourceGroup : IResourceGroup
    {
        private readonly IMainDataRepositoryFactory repoFactory;
        private readonly AppFactory factory;
        private readonly ResourceGroupRecord record;

        internal ResourceGroup(IMainDataRepositoryFactory repoFactory, AppFactory factory, ResourceGroupRecord record)
        {
            this.repoFactory = repoFactory;
            this.factory = factory;
            this.record = record ?? new ResourceGroupRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public ResourceGroupName Name() => new ResourceGroupName(record.Name);

        public async Task<Resource> TryAddResource(ResourceName name)
        {
            var resource = await Resource(name);
            if (!resource.Name().Equals(name))
            {
                resource = await AddResource(name);
            }
            return resource;
        }

        private Task<Resource> AddResource(ResourceName name) => factory.Resources().Add(this, name);

        async Task<IResource> IResourceGroup.Resource(ResourceName name) => await Resource(name);

        public Task<Resource> Resource(ResourceName name) => factory.Resources().Resource(this, name);

        public Task<IEnumerable<Resource>> Resources() => factory.Resources().Resources(this);

        public async Task<IEnumerable<Modifier>> Modifiers()
        {
            var modCategory = await factory.ModCategories().Category(record.ModCategoryID);
            var modifiers = await modCategory.Modifiers();
            return modifiers;
        }

        public async Task<Modifier> Modifier(ModifierKey modKey)
        {
            var modCategory = await factory.ModCategories().Category(record.ModCategoryID);
            var modifier = await modCategory.Modifier(modKey);
            return modifier;
        }

        public Task SetModCategory(ModifierCategory category)
        {
            var repo = repoFactory.CreateResourceGroups();
            return repo.Update
            (
                record, r =>
                {
                    r.ModCategoryID = category.ID.Value;
                }
            );
        }

        async Task<IModifierCategory> IResourceGroup.ModCategory() => await ModCategory();

        public Task<ModifierCategory> ModCategory()
            => factory.ModCategories().Category(record.ModCategoryID);

        public Task AllowAnonymous() => setIsAnonymousAllowed(true);
        public Task DenyAnonymous() => setIsAnonymousAllowed(false);
        private Task setIsAnonymousAllowed(bool isAllowed)
            => repoFactory.CreateResourceGroups()
                .Update
                (
                    record,
                    r =>
                    {
                        r.IsAnonymousAllowed = isAllowed;
                    }
                );

        public Task<IEnumerable<AppRole>> AllowedRoles()
            => factory.Roles().AllowedRolesForResourceGroup(this);

        public Task<IEnumerable<AppRole>> DeniedRoles()
            => factory.Roles().DeniedRolesForResourceGroup(this);

        public Task SetRoleAccess(IEnumerable<AppRole> allowedRoles, IEnumerable<AppRole> deniedRoles)
            => repoFactory.Transaction(() => setRoleAccess(allowedRoles, deniedRoles));

        private async Task setRoleAccess(IEnumerable<AppRole> allowedRoles, IEnumerable<AppRole> deniedRoles)
        {
            await deleteExistingRoles(allowedRoles, deniedRoles);
            var existingAllowedRoles = await AllowedRoles();
            foreach (var allowedRole in allowedRoles)
            {
                if (!existingAllowedRoles.Any(r => r.ID.Equals(allowedRole.ID.Value)))
                {
                    await addGroupRole(allowedRole, true);
                }
            }
            var existingDeniedRoles = await DeniedRoles();
            foreach (var deniedRole in deniedRoles)
            {
                if (!existingDeniedRoles.Any(r => r.ID.Equals(deniedRole.ID.Value)))
                {
                    await addGroupRole(deniedRole, false);
                }
            }
        }

        private async Task deleteExistingRoles(IEnumerable<AppRole> allowedRoles, IEnumerable<AppRole> deniedRoles)
        {
            var groupRoles = repoFactory.CreateResourceGroupRoles();
            var allowedRoleIDs = allowedRoles.Select(r => r.ID.Value);
            var deniedRoleIDs = deniedRoles.Select(r => r.ID.Value);
            var rolesToDelete = await groupRoles
                .Retrieve()
                .Where
                (
                    gr => gr.GroupID == ID.Value
                        && (
                            (!allowedRoleIDs.Any(id => id == gr.RoleID) && gr.IsAllowed)
                            || (!deniedRoleIDs.Any(id => id == gr.RoleID) && !gr.IsAllowed)
                        )
                )
                .ToArrayAsync();
            foreach (var groupRole in rolesToDelete)
            {
                await groupRoles.Delete(groupRole);
            }
        }

        private Task addGroupRole(AppRole role, bool isAllowed)
            => repoFactory.CreateResourceGroupRoles()
                .Create
                (
                    new ResourceGroupRoleRecord
                    {
                        GroupID = ID.Value,
                        RoleID = role.ID.Value,
                        IsAllowed = isAllowed
                    }
                );

        public ResourceGroupModel ToModel()
            => new ResourceGroupModel
            {
                ID = ID.Value,
                Name = Name().DisplayText,
                IsAnonymousAllowed = record.IsAnonymousAllowed
            };

        public override string ToString() => $"{nameof(ResourceGroup)} {ID.Value}";
    }
}
