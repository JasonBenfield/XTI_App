using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class FakeAppApiTemplateFactory : IAppApiTemplateFactory
    {
        public AppApiTemplate Create()
        {
            var api = new FakeAppApi(new AppApiSuperUser());
            return api.Template();
        }
    }
}
