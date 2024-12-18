using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Api;

public class AppApiGroupWrapper : IAppApiGroup
{
    private readonly AppApiGroup source;

    protected internal AppApiGroupWrapper(AppApiGroup source)
    {
        this.source = source;
        Configure(source);
        Path = source.Path;
        Access = source.Access;
    }

    protected virtual void Configure(AppApiGroup source) { }

    public XtiPath Path { get; }
    public ResourceAccess Access { get; }

    public IAppApiAction[] Actions() => source.Actions();

    public bool HasAction(string actionName) => source.HasAction(actionName);

    public TAppApiAction Action<TAppApiAction>(string actionName)
        where TAppApiAction : IAppApiAction =>
        source.Action<TAppApiAction>(actionName);

    public AppRoleName[] RoleNames() => source.RoleNames();

    public AppApiGroupTemplate Template() => source.Template();

    public ThrottledLogPath[] ThrottledLogPaths(XtiBasePath xtiBasePath) =>
        source.ThrottledLogPaths(xtiBasePath);

    public ScheduledAppAgendaItemOptions[] ActionSchedules() => source.ActionSchedules();

}