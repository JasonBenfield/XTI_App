using XTI_App.Abstractions;

namespace XTI_App.Fakes
{
    public sealed class FakeAppOptions
    {
        public AppKey AppKey { get; } = new AppKey(FakeInfo.AppKey.Name, AppType.Values.Service);
        public string Title { get; set; } = "Fake Title";
    }
}
