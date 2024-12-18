using XTI_App.Api;

namespace XTI_ConsoleApp;

public class ConsoleAppApiWrapper : AppApiWrapper
{
    protected ConsoleAppApiWrapper(AppApi source)
        : base(source)
    {
        Lifetime = new LifetimeGroup
        (
            source.AddGroup(nameof(Lifetime), ResourceAccess.AllowAuthenticated())
        );
    }

    public LifetimeGroup Lifetime { get; }
}