using System.Collections.Generic;
using System.Threading.Tasks;

namespace XTI_App
{
    public interface IAppUser
    {
        int ID { get; }
        AppUserName UserName();
        Task<IEnumerable<IAppUserRole>> RolesForApp(IApp app);
    }
}
