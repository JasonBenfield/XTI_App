﻿using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class Resource : IResource
    {
        private readonly AppFactory factory;
        private readonly ResourceRecord record;

        internal Resource(AppFactory factory, ResourceRecord record)
        {
            this.factory = factory;
            this.record = record ?? new ResourceRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public ResourceName Name() => new ResourceName(record.Name);

        public Task AllowAnonymous() => setIsAnonymousAllowed(true);
        public Task DenyAnonymous() => setIsAnonymousAllowed(false);
        private Task setIsAnonymousAllowed(bool isAllowed)
            => factory.DB
                .Resources
                .Update
                (
                    record,
                    r =>
                    {
                        r.IsAnonymousAllowed = isAllowed;
                    }
                );

        internal Task UpdateResultType(ResourceResultType resultType)
            => factory.DB
                .Resources
                .Update
                (
                    record,
                    r =>
                    {
                        r.ResultType = resultType.Value;
                    }
                );

        public Task<AppRole[]> AllowedRoles()
            => factory.Roles().AllowedRolesForResource(this);

        public Task<AppRole[]> DeniedRoles()
            => factory.Roles().DeniedRolesForResource(this);

        public Task SetRoleAccess(IEnumerable<AppRole> allowedRoles, IEnumerable<AppRole> deniedRoles)
            => factory.DB.Transaction(() => setRoleAccess(allowedRoles, deniedRoles));

        private async Task setRoleAccess(IEnumerable<AppRole> allowedRoles, IEnumerable<AppRole> deniedRoles)
        {
            await deleteExistingRoles(allowedRoles, deniedRoles);
            var existingAllowedRoles = await AllowedRoles();
            foreach (var allowedRole in allowedRoles)
            {
                if (!existingAllowedRoles.Any(r => r.ID.Equals(allowedRole.ID.Value)))
                {
                    await addResourceRole(allowedRole, true);
                }
            }
            var existingDeniedRoles = await DeniedRoles();
            foreach (var deniedRole in deniedRoles)
            {
                if (!existingDeniedRoles.Any(r => r.ID.Equals(deniedRole.ID.Value)))
                {
                    await addResourceRole(deniedRole, false);
                }
            }
        }

        private async Task deleteExistingRoles(IEnumerable<AppRole> allowedRoles, IEnumerable<AppRole> deniedRoles)
        {
            var allowedRoleIDs = allowedRoles.Select(r => r.ID.Value);
            var deniedRoleIDs = deniedRoles.Select(r => r.ID.Value);
            var rolesToDelete = await factory.DB
                .ResourceRoles
                .Retrieve()
                .Where
                (
                    rr => rr.ResourceID == ID.Value
                        && (
                            (!allowedRoleIDs.Any(id => id == rr.RoleID) && rr.IsAllowed)
                            || (!deniedRoleIDs.Any(id => id == rr.RoleID) && !rr.IsAllowed)
                        )
                )
                .ToArrayAsync();
            foreach (var resourceRole in rolesToDelete)
            {
                await factory.DB.ResourceRoles.Delete(resourceRole);
            }
        }

        private Task addResourceRole(AppRole role, bool isAllowed)
            => factory.DB
                .ResourceRoles
                .Create
                (
                    new ResourceRoleRecord
                    {
                        ResourceID = ID.Value,
                        RoleID = role.ID.Value,
                        IsAllowed = isAllowed
                    }
                );

        public Task<IEnumerable<AppRequestExpandedModel>> MostRecentRequests(int howMany)
            => factory.Requests().MostRecentForResource(this, howMany);

        public Task<IEnumerable<AppEvent>> MostRecentErrorEvents(int howMany)
            => factory.Events().MostRecentErrorsForResource(this, howMany);

        public ResourceModel ToModel()
            => new ResourceModel
            {
                ID = ID.Value,
                Name = Name().DisplayText,
                IsAnonymousAllowed = record.IsAnonymousAllowed,
                ResultType = ResourceResultType.Values.Value(record.ResultType)
            };

        public override string ToString() => $"{nameof(Resource)} {ID.Value}";
    }
}
