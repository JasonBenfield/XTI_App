using System.Threading.Tasks;
using XTI_App.Api;
using XTI_Core;

namespace XTI_App.TestFakes
{
    public sealed class AddEmployeeValidation : AppActionValidation<AddEmployeeModel>
    {
        public Task Validate(ErrorList errors, AddEmployeeModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                errors.Add("Name is required");
            }
            return Task.CompletedTask;
        }
    }

}
