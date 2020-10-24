namespace XTI_App.TestFakes
{
    public static class FakeAppKey
    {
        public static readonly AppName AppName = new AppName("Fake");
        public static readonly AppKey AppKey = new AppKey(AppName, AppType.Values.Service);
    }
}
