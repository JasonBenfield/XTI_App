using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class DemoAppApiFactory : AppApiFactory
{
    private readonly IServiceProvider sp;

    public DemoAppApiFactory(IServiceProvider sp)
    {
        this.sp = sp;
    }

    protected override IAppApi _Create(IAppApiUser user) => new DemoAppApi(user, sp);
}