using System.Collections.Generic;
using System.Threading.Tasks;

namespace XTI_App.Abstractions
{
    public interface IAppUser
    {
        EntityID ID { get; }
        AppUserName UserName();
        Task<IAppRole[]> Roles(IModifier modifier);
    }
}
