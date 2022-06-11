using XTI_App.Abstractions;

namespace XTI_App.Api;

public class AppApiGroupWrapper : IAppApiGroup
{
    protected readonly AppApiGroup source;

    protected AppApiGroupWrapper(AppApiGroup source)
    {
        this.source = source;
    }

    public XtiPath Path { get => source.Path; }
    public ResourceAccess Access { get => source.Access; }
    public Task<bool> HasAccess() => source.HasAccess();
    public IEnumerable<IAppApiAction> Actions() => source.Actions();
    public TAppApiAction Action<TAppApiAction>(string actionName) 
        where TAppApiAction : IAppApiAction => source.Action<TAppApiAction>(actionName);
    public AppApiGroupTemplate Template() => source.Template();
}