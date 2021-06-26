using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class ResourceGroup : IResourceGroup
    {
        private readonly AppFactory factory;
        private readonly ResourceGroupRecord record;

        internal ResourceGroup(AppFactory factory, ResourceGroupRecord record)
        {
            this.factory = factory;
            this.record = record ?? new ResourceGroupRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public ResourceGroupName Name() => new ResourceGroupName(record.Name);

        public async Task<Resource> TryAddResource(ResourceName name, ResourceResultType resultType)
        {
            var resource = await Resource(name);
            if (resource.Name().Equals(name))
            {
                await resource.UpdateResultType(resultType);
            }
            else
            {
                resource = await AddResource(name, resultType);
            }
            return resource;
        }

        private Task<Resource> AddResource(ResourceName name, ResourceResultType resultType)
            => factory.Resources().Add(this, name, resultType);

        async Task<IResource> IResourceGroup.Resource(ResourceName name) => await Resource(name);

        public Task<Resource> Resource(ResourceName name) => factory.Resources().Resource(this, name);

        public Task<Resource[]> Resources() => factory.Resources().Resources(this);

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
            => factory.DB
                .ResourceGroups
                .Update
                (
                    record, r =>
                    {
                        r.ModCategoryID = category.ID.Value;
                    }
                );

        async Task<IModifierCategory> IResourceGroup.ModCategory() => await ModCategory();

        public Task<ModifierCategory> ModCategory()
            => factory.ModCategories().Category(record.ModCategoryID);

        public Task AllowAnonymous() => setIsAnonymousAllowed(true);
        public Task DenyAnonymous() => setIsAnonymousAllowed(false);
        private Task setIsAnonymousAllowed(bool isAllowed)
            => factory.DB
                .ResourceGroups
                .Update
                (
                    record,
                    r =>
                    {
                        r.IsAnonymousAllowed = isAllowed;
                    }
                );

        public Task<AppRole[]> AllowedRoles()
            => factory.Roles().AllowedRolesForResourceGroup(this);

        public Task SetRoleAccess(IEnumerable<AppRole> allowedRoles)
            => factory.DB.Transaction(() => setRoleAccess(allowedRoles));

        private async Task setRoleAccess(IEnumerable<AppRole> allowedRoles)
        {
            await deleteExistingRoles(allowedRoles);
            var existingAllowedRoles = await AllowedRoles();
            foreach (var allowedRole in allowedRoles)
            {
                if (!existingAllowedRoles.Any(r => r.ID.Equals(allowedRole.ID.Value)))
                {
                    await addGroupRole(allowedRole, true);
                }
            }
        }

        private async Task deleteExistingRoles(IEnumerable<AppRole> allowedRoles)
        {
            var allowedRoleIDs = allowedRoles.Select(r => r.ID.Value);
            var rolesToDelete = await factory.DB
                .ResourceGroupRoles
                .Retrieve()
                .Where
                (
                    gr => gr.GroupID == ID.Value
                        &&
                        (
                            !allowedRoleIDs.Any(id => id == gr.RoleID) && gr.IsAllowed
                        )
                )
                .ToArrayAsync();
            foreach (var groupRole in rolesToDelete)
            {
                await factory.DB.ResourceGroupRoles.Delete(groupRole);
            }
        }

        private Task addGroupRole(AppRole role, bool isAllowed)
            => factory.DB
                .ResourceGroupRoles
                .Create
                (
                    new ResourceGroupRoleRecord
                    {
                        GroupID = ID.Value,
                        RoleID = role.ID.Value,
                        IsAllowed = isAllowed
                    }
                );

        public Task<IEnumerable<AppRequestExpandedModel>> MostRecentRequests(int howMany)
            => factory.Requests().MostRecentForResourceGroup(this, howMany);

        public Task<IEnumerable<AppEvent>> MostRecentErrorEvents(int howMany)
            => factory.Events().MostRecentErrorsForResourceGroup(this, howMany);

        public ResourceGroupModel ToModel()
            => new ResourceGroupModel
            {
                ID = ID.Value,
                Name = Name().DisplayText,
                IsAnonymousAllowed = record.IsAnonymousAllowed,
                ModCategoryID = record.ModCategoryID
            };

        public override string ToString() => $"{nameof(ResourceGroup)} {ID.Value}";
    }
}
