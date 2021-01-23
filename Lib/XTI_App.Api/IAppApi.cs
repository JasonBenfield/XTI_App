using System.Collections.Generic;
using System.Threading.Tasks;

namespace XTI_App.Api
{
    public interface IAppApi
    {
        XtiPath Path { get; }
        AppKey AppKey { get; }
        ResourceAccess Access { get; }
        Task<bool> HasAccess();
        IEnumerable<IAppApiGroup> Groups();
        IAppApiGroup Group(string groupName);
        AppApiTemplate Template();
    }
}
