using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class ModifierCategoryAdminRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<ModifierCategoryAdminRecord> repo;

        internal ModifierCategoryAdminRepository(AppFactory factory, DataRepository<ModifierCategoryAdminRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        internal Task Add(ModifierCategory modCategory, AppUser user)
        {
            var record = new ModifierCategoryAdminRecord
            {
                ModCategoryID = modCategory.ID.Value,
                UserID = user.ID.Value
            };
            return repo.Create(record);
        }

        internal Task<bool> IsAdmin(IModifierCategory modCategory, IAppUser user)
            => repo.Retrieve()
                .AnyAsync(a => a.ModCategoryID == modCategory.ID.Value && a.UserID == user.ID.Value);

        internal async Task Delete(ModifierCategory modCategory, AppUser user)
        {
            var record = await repo.Retrieve()
                .FirstOrDefaultAsync(a => a.ModCategoryID == modCategory.ID.Value && a.UserID == user.ID.Value);
            if (record != null)
            {
                await repo.Delete(record);
            }
        }
    }
}
