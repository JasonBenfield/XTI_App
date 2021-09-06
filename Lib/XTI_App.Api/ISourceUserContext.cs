using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public interface ISourceUserContext : IUserContext
    {
        Task<IAppUser> User(AppUserName userName);
    }
}
