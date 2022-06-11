using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class EmployeeGroup : AppApiGroupWrapper
{
    public EmployeeGroup(AppApiGroup source) : base(source)
    {
        AddEmployee = source.AddAction
        (
            nameof(AddEmployee),
            () => new AddEmployeeAction(),
            () => new AddEmployeeValidation(),
            source.Access.WithAllowed(FakeAppRoles.Instance.Manager)
        );
        Employee = source.AddAction
        (
            nameof(Employee),
            () => new EmployeeAction(),
            access: source.Access.WithAllowed(FakeAppRoles.Instance.Viewer),
            friendlyName: "Get Employee Information"
        );
        SubmitFakeForm = source.AddAction
        (
            nameof(SubmitFakeForm), 
            () => new SubmitFakeFormAction()
        );
    }
    public AppApiAction<AddEmployeeModel, int> AddEmployee { get; }
    public AppApiAction<int, Employee> Employee { get; }
    public AppApiAction<FakeForm, string> SubmitFakeForm { get; }
}