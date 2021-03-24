using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class ModifierRepository
    {
        private readonly AppFactory factory;

        internal ModifierRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        internal async Task<Modifier> Add(ModifierCategory category, ModifierKey modKey, string targetID, string displayText)
        {
            var record = new ModifierRecord
            {
                CategoryID = category.ID.Value,
                ModKey = modKey.Value,
                TargetKey = targetID,
                DisplayText = displayText
            };
            await factory.DB.Modifiers.Create(record);
            return factory.Modifier(record);
        }

        internal async Task<IEnumerable<Modifier>> Modifiers(ModifierCategory category)
        {
            var records = await modifiersForCategory(category).ToArrayAsync();
            return records.Select(m => factory.Modifier(m));
        }

        internal async Task<Modifier> Modifier(int modifierID)
        {
            var record = await factory.DB
                .Modifiers
                .Retrieve()
                .Where(m => m.ID == modifierID)
                .FirstOrDefaultAsync();
            return factory.Modifier(record);
        }

        internal async Task<Modifier> Modifier(ModifierKey modKey)
        {
            var record = await factory.DB
                .Modifiers
                .Retrieve()
                .Where(m => m.ModKey == modKey.Value)
                .FirstOrDefaultAsync();
            if (record == null)
            {
                record = await factory.DB
                    .Modifiers
                    .Retrieve()
                    .Where(m => m.ModKey == ModifierKey.Default.Value)
                    .FirstOrDefaultAsync();
            }
            return factory.Modifier(record);
        }

        internal async Task<Modifier> Modifier(ModifierCategory category, string targetKey)
        {
            var record = await modifiersForCategory(category)
                .Where(m => m.TargetKey == targetKey)
                .FirstOrDefaultAsync();
            return factory.Modifier(record);
        }

        internal async Task<IEnumerable<Modifier>> ModifiersNotAssignedToUser(AppUser appUser, ModifierCategory modCategory)
        {
            var modIDs = modIDsForUser(appUser);
            var records = await modifiersForCategory(modCategory)
                .Where(m => !modIDs.Any(id => m.ID == id))
                .ToArrayAsync();
            return records.Select(r => factory.Modifier(r));
        }

        internal async Task<IEnumerable<Modifier>> ModifiersAssignedToUser(AppUser appUser, ModifierCategory modCategory)
        {
            var modIDs = modIDsForUser(appUser);
            var records = await modifiersForCategory(modCategory)
                .Where(m => modIDs.Any(id => m.ID == id))
                .ToArrayAsync();
            return records.Select(r => factory.Modifier(r));
        }

        private IQueryable<ModifierRecord> modifiersForCategory(ModifierCategory modCategory)
        {
            return factory.DB
                .Modifiers
                .Retrieve()
                .Where(m => m.CategoryID == modCategory.ID.Value);
        }

        private IQueryable<int> modIDsForUser(AppUser appUser)
        {
            return factory.DB
                .UserModifiers
                .Retrieve()
                .Where(um => um.UserID == appUser.ID.Value)
                .Select(um => um.ModifierID);
        }

    }
}
