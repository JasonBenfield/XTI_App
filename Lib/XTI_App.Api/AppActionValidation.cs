using System.Threading.Tasks;

namespace XTI_App.Api
{
    public interface AppActionValidation<TModel>
    {
        Task Validate(ErrorList errors, TModel model);
    }
}
