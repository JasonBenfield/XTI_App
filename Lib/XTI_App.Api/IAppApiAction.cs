using System.Threading.Tasks;

namespace XTI_App.Api
{
    public interface IAppApiAction
    {
        XtiPath Path { get; }
        string FriendlyName { get; }
        ResourceAccess Access { get; }

        Task<bool> HasAccess(ModifierKey modifier);

        Task<object> Execute(ModifierKey modifier, object model);

        AppApiActionTemplate Template();
    }
}
