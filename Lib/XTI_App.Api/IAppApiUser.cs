using System.Threading.Tasks;

namespace XTI_App.Api
{
    public interface IAppApiUser
    {
        Task<bool> HasAccessToApp();
        Task<bool> HasAccess(ResourceAccess resourceAccess);
    }
}
