using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class FakeAppApiFactory : AppApiFactory
    {
        protected override AppApi _Create(IAppApiUser user) => new FakeAppApi(new AppApiSuperUser());
    }
}
