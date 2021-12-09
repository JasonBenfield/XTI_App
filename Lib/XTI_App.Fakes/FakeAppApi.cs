using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeAppApi : AppApiWrapper
{
    public FakeAppApi(IAppApiUser user, IServiceProvider sp)
        : base
        (
            new AppApi
            (
                FakeInfo.AppKey,
                user,
                ResourceAccess.AllowAuthenticated()
                    .WithAllowed(AppRoleName.Admin)
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
                nameof(Product)
            )
        );
        Agenda = new AgendaGroup
        (
            source.AddGroup(nameof(Agenda), ResourceAccess.AllowAnonymous()),
            sp
        );
    }
    public HomeGroup Home { get; }
    public LoginGroup Login { get; }
    public EmployeeGroup Employee { get; }
    public ProductGroup Product { get; }
    public AgendaGroup Agenda { get; }
}