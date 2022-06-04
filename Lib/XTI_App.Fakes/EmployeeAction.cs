using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class EmployeeAction : AppAction<int, Employee>
{
    public Task<Employee> Execute(int id, CancellationToken stoppingToken)
    {
        return Task.FromResult(new Employee { ID = id, Name = "Someone", BirthDate = DateTime.Today });
    }
}