using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;
using System.Collections.Generic;
using System.Linq;

namespace XTI_App
{
    public sealed class AppUserModifierRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppUserModifierRecord> repo;

        internal AppUserModifierRepository(AppFactory factory, DataRepository<AppUserModifierRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        internal Task Add(AppUser user, Modifier modifier)
        {
            var record = new AppUserModifierRecord
            {
                UserID = user.ID.Value,
                ModifierID = modifier.ID.Value
            };
            return repo.Create(record);
        }

        internal async Task Delete(AppUser user, Modifier modifier)
        {
            var record = await repo.Retrieve()
                .FirstOrDefaultAsync(um => um.UserID == user.ID.Value && um.ModifierID == modifier.ID.Value);
            if (record != null)
            {
                await repo.Delete(record);
            }
        }

        internal Task<bool> Any(AppUser appUser, Modifier modifier)
        {
            return repo.Retrieve()
                .AnyAsync(um => um.UserID == appUser.ID.Value && um.ModifierID == modifier.ID.Value);
        }

        internal async Task<IEnumerable<Modifier>> Modifiers(AppUser appUser, ModifierCategory modCategory)
        {
            var records = await repo.Retrieve()
                .Where(um => um.UserID == appUser.ID.Value)
                .ToArrayAsync();
            var userModifiers = records.Select(r => factory.UserModifier(r));
            var modifiers = new List<Modifier>();
            foreach (var userModifier in userModifiers)
            {
                var modifier = await userModifier.Modifier();
                if (modifier.IsForCategory(modCategory))
                {
                    modifiers.Add(modifier);
                }
            }
            return modifiers;
        }
    }
}
