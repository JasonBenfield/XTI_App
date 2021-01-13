using XTI_App.Api;
using XTI_App.Tests;

namespace XTI_App.TestFakes
{
    public sealed class EmployeeGroup : AppApiGroup
    {
        public EmployeeGroup(AppApi api, IAppApiUser user)
            : base
            (
                  api,
                  new NameFromGroupClassName(nameof(EmployeeGroup)).Value,
                  new ModifierCategoryName("Department"),
                  api.Access,
                  user,
                  (n, a, u) => new AppApiActionCollection(n, a, u)
            )
        {
            var actions = Actions<AppApiActionCollection>();
            AddEmployee = actions.Add
            (
                nameof(AddEmployee),
                api.Access.WithAllowed(FakeAppRoles.Instance.Manager),
                () => new AddEmployeeValidation(),
                () => new AddEmployeeAction()
            );
            Employee = actions.Add
            (
                nameof(Employee),
                () => new EmployeeAction(),
                "Get Employee Information"
            );
            SubmitFakeForm = actions.Add(nameof(SubmitFakeForm), () => new SubmitFakeFormAction());
        }
        public AppApiAction<AddEmployeeModel, int> AddEmployee { get; }
        public AppApiAction<int, Employee> Employee { get; }
        public AppApiAction<FakeForm, string> SubmitFakeForm { get; }
    }

}
