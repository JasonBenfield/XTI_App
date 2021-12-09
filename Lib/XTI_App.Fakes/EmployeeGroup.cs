using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class EmployeeGroup : AppApiGroupWrapper
{
    public EmployeeGroup(AppApiGroup source) : base(source)
    {
        var actions = new AppApiActionFactory(source);
        AddEmployee = source.AddAction
        (
            actions.Action
            (
                nameof(AddEmployee),
                source.Access.WithAllowed(FakeAppRoles.Instance.Manager),
                () => new AddEmployeeValidation(),
                () => new AddEmployeeAction()
            )
        );
        Employee = source.AddAction
        (
            actions.Action
            (
                nameof(Employee),
                source.Access.WithAllowed(FakeAppRoles.Instance.Viewer),
                () => new EmployeeAction(),
                "Get Employee Information"
            )
        );
        SubmitFakeForm = source.AddAction
        (
            actions.Action(nameof(SubmitFakeForm), () => new SubmitFakeFormAction())
        );
    }
    public AppApiAction<AddEmployeeModel, int> AddEmployee { get; }
    public AppApiAction<int, Employee> Employee { get; }
    public AppApiAction<FakeForm, string> SubmitFakeForm { get; }
}