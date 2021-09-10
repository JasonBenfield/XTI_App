using System;
using System.Threading.Tasks;
using XTI_App.Api;

namespace XTI_App.Fakes
{
    public sealed class EmployeeAction : AppAction<int, Employee>
    {
        public Task<Employee> Execute(int id)
        {
            return Task.FromResult(new Employee { ID = id, Name = "Someone", BirthDate = DateTime.Today });
        }
    }

}
