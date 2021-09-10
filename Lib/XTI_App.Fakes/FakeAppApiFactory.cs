using XTI_App.Api;

namespace XTI_App.Fakes
{
    public sealed class FakeAppApiFactory : AppApiFactory
    {
        protected override IAppApi _Create(IAppApiUser user) => new FakeAppApi(user);
    }
}
