using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Extensions
{
    internal sealed class CachedAppUser : IAppUser
    {
        private readonly IServiceProvider services;
        private readonly AppUserName userName;

        public CachedAppUser(IServiceProvider services, IAppUser source)
        {
            this.services = services;
            ID = source.ID;
            userName = source.UserName();
        }

        public EntityID ID { get; }

        public AppUserName UserName() => userName;

        private readonly ConcurrentDictionary<string, CachedAppRole[]> cachedModRoles = new ConcurrentDictionary<string, CachedAppRole[]>();

        public async Task<IEnumerable<IAppRole>> Roles(IModifier modifier)
        {
            var key = $"{modifier.ID.Value}";
            if (!cachedModRoles.TryGetValue(key, out var cachedAssignedRoles))
            {
                var factory = services.GetService<AppFactory>();
                var user = await factory.Users().User(ID.Value);
                var assignedRoles = await user.AssignedRoles(modifier);
                cachedAssignedRoles = assignedRoles
                    .Select(ur => new CachedAppRole(ur))
                    .ToArray();
                cachedModRoles.AddOrUpdate(key, cachedAssignedRoles, (k, r) => cachedAssignedRoles);
            }
            return cachedAssignedRoles;
        }
    }
}
