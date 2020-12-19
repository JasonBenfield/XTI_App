namespace XTI_App.TestFakes
{
    public sealed class FakeAppOptions
    {
        public AppKey AppKey { get; } = new AppKey(FakeAppKey.AppKey.Name, AppType.Values.Service);
        public string Title { get; set; } = "Fake Title";
        public AppRoleNames RoleNames { get; } = FakeAppRoles.Instance;
    }
}
