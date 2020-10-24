using System.Threading.Tasks;

namespace XTI_App.Api
{
    public interface IAppApiUser
    {
        Task<bool> HasAccessToApp(XtiPath path);
        Task<bool> HasAccess(XtiPath path, ResourceAccess resourceAccess, ModifierKey modKey);
    }
}
