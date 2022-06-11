using XTI_App.Abstractions;

namespace XTI_App.Api;

public interface IAppApiGroup
{
    XtiPath Path { get; }
    ResourceAccess Access { get; }
    Task<bool> HasAccess();
    IEnumerable<IAppApiAction> Actions();
    TAppApiAction Action<TAppApiAction>(string actionName) where TAppApiAction : IAppApiAction;
    AppApiGroupTemplate Template();
}