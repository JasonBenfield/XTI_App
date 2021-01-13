using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class FakeAppApi : AppApi
    {
        public FakeAppApi(IAppApiUser user)
            : base
            (
                FakeAppKey.AppKey,
                user,
                ResourceAccess.AllowAuthenticated()
                    .WithAllowed(FakeAppRoles.Instance.Admin)
            )
        {
            Home = AddGroup(u => new HomeGroup(this, u));
            Login = AddGroup(u => new LoginGroup(this, u));
            Employee = AddGroup(u => new EmployeeGroup(this, u));
            Product = AddGroup(u => new ProductGroup(this, u));
        }
        public HomeGroup Home { get; }
        public LoginGroup Login { get; }
        public EmployeeGroup Employee { get; }
        public ProductGroup Product { get; }
    }
}
