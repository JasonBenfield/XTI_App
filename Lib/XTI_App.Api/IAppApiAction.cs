using System.Threading.Tasks;

namespace XTI_App.Api
{
    public interface IAppApiAction
    {
        string ActionName { get; }
        string FriendlyName { get; }
        ResourceAccess Access { get; }

        Task<bool> HasAccess();

        AppApiActionTemplate Template();
    }
}
