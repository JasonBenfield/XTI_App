using System.Threading.Tasks;
using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class AddEmployeeAction : AppAction<AddEmployeeModel, int>
    {
        public Task<int> Execute(AddEmployeeModel model)
        {
            return Task.FromResult(1);
        }
    }

}
