using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Api;

public interface IAppApiGroup
{
    XtiPath Path { get; }
    ResourceAccess Access { get; }
    IAppApiAction[] Actions();
    TAppApiAction Action<TAppApiAction>(string actionName) where TAppApiAction : IAppApiAction;
    AppApiGroupTemplate Template();
    AppRoleName[] RoleNames();
    ThrottledLogPath[] ThrottledLogPaths(XtiBasePath xtiBasePath);
}