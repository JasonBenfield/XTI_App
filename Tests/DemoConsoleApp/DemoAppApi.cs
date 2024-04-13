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
                ResourceAccess.AllowAuthenticated(),
                ""
            ),
            sp
        )
    {
        PreDemo = new PreDemoGroup(source.AddGroup(nameof(PreDemo)));
        Demo = new DemoGroup(source.AddGroup(nameof(Demo)));
        PostDemo = new PostDemoGroup(source.AddGroup(nameof(PostDemo)));
    }

    public PreDemoGroup PreDemo { get; }
    public DemoGroup Demo { get; }
    public PostDemoGroup PostDemo { get; }
}