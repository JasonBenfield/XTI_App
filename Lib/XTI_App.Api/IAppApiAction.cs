using XTI_App.Abstractions;

namespace XTI_App.Api;

public interface IAppApiAction
{
    string ActionName { get; }
    XtiPath Path { get; }
    string FriendlyName { get; }
    ResourceAccess Access { get; }

    AppApiActionTemplate Template();
}