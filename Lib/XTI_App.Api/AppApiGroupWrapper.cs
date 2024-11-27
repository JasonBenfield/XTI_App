using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Api;

public class AppApiGroupWrapper : IAppApiGroup
{
    private readonly AppApiGroup source;

    protected AppApiGroupWrapper(AppApiGroup source)
    {
        this.source = source;
        Configure(source);
    }

    protected virtual void Configure(AppApiGroup source) { }

    public XtiPath Path { get => source.Path; }
    public ResourceAccess Access { get => source.Access; }

    public Task<bool> HasAccess() => source.HasAccess();

    public IAppApiAction[] Actions() => source.Actions();

    public TAppApiAction Action<TAppApiAction>(string actionName)
        where TAppApiAction : IAppApiAction =>
        source.Action<TAppApiAction>(actionName);

    public AppRoleName[] RoleNames() => source.RoleNames();

    public AppApiGroupTemplate Template() => source.Template();

    public ThrottledLogPath[] ThrottledLogPaths() => source.ThrottledLogPaths();
}