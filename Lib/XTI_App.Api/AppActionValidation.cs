using System.Threading.Tasks;
using XTI_Core;

namespace XTI_App.Api
{
    public interface AppActionValidation<TModel>
    {
        Task Validate(ErrorList errors, TModel model);
    }
}
