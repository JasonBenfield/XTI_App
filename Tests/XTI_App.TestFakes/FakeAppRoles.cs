namespace XTI_App.TestFakes
{
    public sealed class FakeAppRoles : AppRoleNames
    {
        public static readonly FakeAppRoles Instance = new FakeAppRoles();

        public FakeAppRoles()
        {
            Admin = Add("Admin");
            Viewer = Add("Viewer");
            Manager = Add("Manager");
        }
        public AppRoleName Admin { get; }
        public AppRoleName Viewer { get; }
        public AppRoleName Manager { get; }
    }
}
