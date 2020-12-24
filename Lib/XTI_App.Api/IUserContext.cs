using System.Threading.Tasks;

namespace XTI_App.Api
{
    public interface IUserContext
    {
        void RefreshUser(IAppUser user);
        Task<IAppUser> User();
        Task<AppUser> UncachedUser();
    }
}
