using XTI_App.Api;
using XTI_ConsoleApp;

namespace DemoConsoleApp;

public sealed class DemoAppApi : ConsoleAppApiWrapper
{
    public DemoAppApi(IAppApiUser user, IServiceProvider sp)
        : base
        (
            new AppApi
            (
                DemoInfo.AppKey,
                user,
                ResourceAccess.AllowAuthenticated()
            ),
            sp
        )
    {
        Demo = new DemoGroup(source.AddGroup(nameof(Demo)));
    }

    public DemoGroup Demo { get; }
}