using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class ModifierCategoryRepository
    {
        private readonly AppFactory factory;

        internal ModifierCategoryRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        internal async Task<ModifierCategory> Add(IApp app, ModifierCategoryName name)
        {
            var record = new ModifierCategoryRecord
            {
                AppID = app.ID.Value,
                Name = name.Value
            };
            await factory.DB.ModifierCategories.Create(record);
            return factory.ModCategory(record);
        }

        internal async Task<ModifierCategory> Category(int id)
        {
            var record = await factory.DB
                .ModifierCategories
                .Retrieve()
                .FirstOrDefaultAsync(c => c.ID == id);
            return factory.ModCategory(record);
        }

        internal Task<ModifierCategory[]> Categories(IApp app)
        {
            return factory.DB
                .ModifierCategories
                .Retrieve()
                .Where(c => c.AppID == app.ID.Value)
                .OrderBy(c => c.Name)
                .Select(c => factory.ModCategory(c))
                .ToArrayAsync();
        }

        internal async Task<ModifierCategory> Category(App app, int id)
        {
            var record = await factory.DB
                .ModifierCategories
                .Retrieve()
                .FirstOrDefaultAsync(c => c.AppID == app.ID.Value && c.ID == id);
            return factory.ModCategory(record);
        }

        internal async Task<ModifierCategory> Category(IApp app, ModifierCategoryName name)
        {
            var record = await factory.DB
                .ModifierCategories
                .Retrieve()
                .FirstOrDefaultAsync(c => c.AppID == app.ID.Value && c.Name == name.Value);
            if (record == null)
            {
                record = await factory.DB
                    .ModifierCategories
                    .Retrieve()
                    .FirstOrDefaultAsync(c => c.AppID == app.ID.Value && c.Name == ModifierCategoryName.Default.Value);
            }
            return factory.ModCategory(record);
        }
    }
}
