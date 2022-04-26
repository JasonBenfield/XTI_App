using XTI_App.Api;

namespace XTI_ConsoleApp;

public class ConsoleAppApiWrapper : AppApiWrapper
{
    protected ConsoleAppApiWrapper(AppApi source, IServiceProvider sp)
        : base(source)
    {
        Lifetime = new LifetimeGroup
        (
            source.AddGroup(nameof(Lifetime), ResourceAccess.AllowAuthenticated()),
            sp
        );
    }

    public LifetimeGroup Lifetime { get; }
}