using XTI_App.Abstractions;

namespace XTI_App.TestFakes
{
    public sealed class FakeAppRoles
    {
        public static readonly FakeAppRoles Instance = new FakeAppRoles();

        public AppRoleName Admin { get; } = new AppRoleName(nameof(Admin));
        public AppRoleName Viewer { get; } = new AppRoleName(nameof(Viewer));
        public AppRoleName Manager { get; } = new AppRoleName(nameof(Manager));
    }
}
