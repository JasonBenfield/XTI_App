using System.Collections.Generic;
using System.Threading.Tasks;

namespace XTI_App.Abstractions
{
    public interface IAppUser
    {
        EntityID ID { get; }
        AppUserName UserName();
        Task<IEnumerable<IAppRole>> Roles(IApp app);
        Task<bool> IsModCategoryAdmin(IModifierCategory modCategory);
        Task<bool> HasModifier(ModifierKey modKey);
    }
}
