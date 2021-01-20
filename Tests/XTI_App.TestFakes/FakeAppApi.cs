using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class FakeAppApi : AppApiWrapper
    {
        public FakeAppApi(IAppApiUser user)
            : base
            (
                new AppApi
                (
                    FakeInfo.AppKey,
                    user,
                    ResourceAccess.AllowAuthenticated()
                        .WithAllowed(FakeAppRoles.Instance.Admin)
                )
            )
        {
            Home = new HomeGroup
            (
                source.AddGroup
                (
                    nameof(Home),
                    ResourceAccess.AllowAuthenticated()
                )
            );
            Login = new LoginGroup
            (
                source.AddGroup
                (
                    nameof(Login),
                    ResourceAccess.AllowAnonymous()
                )
            );
            Employee = new EmployeeGroup
            (
                source.AddGroup
                (
                    nameof(Employee),
                    FakeInfo.ModCategories.Department
                )
            );
            Product = new ProductGroup
            (
                source.AddGroup
                (
                    nameof(Product),
                    Access.WithDenied(FakeAppRoles.Instance.Viewer)
                )
            );
        }
        public HomeGroup Home { get; }
        public LoginGroup Login { get; }
        public EmployeeGroup Employee { get; }
        public ProductGroup Product { get; }
    }
}
