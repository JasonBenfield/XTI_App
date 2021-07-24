using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
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

        internal Task<Modifier[]> Modifiers(ModifierCategory category)
        {
            return modifiersForCategory(category)
                .Select(m => factory.Modifier(m))
                .ToArrayAsync();
        }

        public async Task<Modifier> Modifier(int modifierID)
        {
            var record = await factory.DB
                .Modifiers
                .Retrieve()
                .Where(m => m.ID == modifierID)
                .FirstOrDefaultAsync();
            return factory.Modifier(record);
        }

        internal async Task<Modifier> Modifier(ModifierCategory modCategory, ModifierKey modKey)
        {
            if
            (
                !modCategory.Name().Equals(ModifierCategoryName.Default)
                && modKey.Equals(ModifierKey.Default)
            )
            {
                var app = await modCategory.App();
                modCategory = await app.ModCategory(ModifierCategoryName.Default);
            }
            var record = await factory.DB
                .Modifiers
                .Retrieve()
                .Where(m => m.CategoryID == modCategory.ID.Value && m.ModKey == modKey.Value)
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

        private IQueryable<ModifierRecord> modifiersForCategory(ModifierCategory modCategory)
        {
            return factory.DB
                .Modifiers
                .Retrieve()
                .Where(m => m.CategoryID == modCategory.ID.Value);
        }
    }
}
