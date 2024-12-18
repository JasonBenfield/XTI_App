using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class EmployeeGroup : AppApiGroupWrapper
{
    public EmployeeGroup(AppApiGroup source) : base(source)
    {
        AddEmployee = source.AddAction<AddEmployeeModel, int>()
            .Named(nameof(AddEmployee))
            .WithExecution(() => new AddEmployeeAction())
            .WithValidation(() => new AddEmployeeValidation())
            .WithAllowed(FakeAppRoles.Instance.Manager)
            .Build();
        Employee = source.AddAction<int, Employee>()
            .Named(nameof(Employee))
            .WithExecution(() => new EmployeeAction())
            .WithAllowed(FakeAppRoles.Instance.Viewer)
            .WithFriendlyName("Get Employee Information")
            .Build();;
        SubmitFakeForm = source.AddAction<FakeForm, string>()
            .Named(nameof(SubmitFakeForm))
            .WithExecution(() => new SubmitFakeFormAction())
            .Build();
    }
    public AppApiAction<AddEmployeeModel, int> AddEmployee { get; }
    public AppApiAction<int, Employee> Employee { get; }
    public AppApiAction<FakeForm, string> SubmitFakeForm { get; }
}