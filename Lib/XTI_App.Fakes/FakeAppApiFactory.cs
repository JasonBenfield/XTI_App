using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeAppApiFactory : AppApiFactory
{
    private readonly IServiceProvider sp;

    public FakeAppApiFactory(IServiceProvider sp)
    {
        this.sp = sp;
    }

    protected override IAppApi _Create(IAppApiUser user) => new FakeAppApi(user, sp);

    public new FakeAppApi CreateForSuperUser() => (FakeAppApi)base.CreateForSuperUser();

    public new FakeAppApi Create(IAppApiUser user) => (FakeAppApi)base.Create(user);
}