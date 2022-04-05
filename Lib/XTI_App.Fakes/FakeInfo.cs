using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public static class FakeInfo
{
    public static readonly AppKey AppKey = new AppKey(new AppName("Fake"), AppType.Values.ServiceApp);
    public static readonly FakeAppRoles Roles = FakeAppRoles.Instance;
    public static readonly FakeModCategoryNames ModCategories = FakeModCategoryNames.Instance;
}