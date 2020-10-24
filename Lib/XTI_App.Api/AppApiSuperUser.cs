using System.Threading.Tasks;

namespace XTI_App.Api
{
    public sealed class AppApiSuperUser : IAppApiUser
    {
        public Task<bool> HasAccessToApp(XtiPath path)
        {
            return Task.FromResult(true);
        }

        public Task<bool> HasAccess(XtiPath path, ResourceAccess resourceAccess, ModifierKey modKey)
        {
            return Task.FromResult(true);
        }
    }
}
