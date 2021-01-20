namespace XTI_App.TestFakes
{
    public sealed class FakeAppOptions
    {
        public AppKey AppKey { get; } = new AppKey(FakeInfo.AppKey.Name, AppType.Values.Service);
        public string Title { get; set; } = "Fake Title";
    }
}
