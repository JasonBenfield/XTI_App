using System.Threading.Tasks;

namespace XTI_App.Api
{
    public interface IAppApiAction
    {
        XtiPath Name { get; }
        string FriendlyName { get; }
        ResourceAccess Access { get; }

        Task<bool> HasAccess(AccessModifier modifier);

        Task<object> Execute(AccessModifier modifier, object model);

        AppApiActionTemplate Template();
    }
}
