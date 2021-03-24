using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class ModifierCategoryAdminRepository
    {
        private readonly AppFactory factory;

        internal ModifierCategoryAdminRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        internal Task Add(ModifierCategory modCategory, AppUser user)
        {
            var record = new ModifierCategoryAdminRecord
            {
                ModCategoryID = modCategory.ID.Value,
                UserID = user.ID.Value
            };
            return factory.DB.ModifierCategoryAdmins.Create(record);
        }

        internal Task<bool> IsAdmin(IModifierCategory modCategory, IAppUser user)
            => factory.DB
                .ModifierCategoryAdmins
                .Retrieve()
                .AnyAsync(a => a.ModCategoryID == modCategory.ID.Value && a.UserID == user.ID.Value);

        internal async Task Delete(ModifierCategory modCategory, AppUser user)
        {
            var record = await factory.DB
                .ModifierCategoryAdmins
                .Retrieve()
                .FirstOrDefaultAsync(a => a.ModCategoryID == modCategory.ID.Value && a.UserID == user.ID.Value);
            if (record != null)
            {
                await factory.DB.ModifierCategoryAdmins.Delete(record);
            }
        }
    }
}
