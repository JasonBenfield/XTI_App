using Microsoft.AspNetCore.Mvc.ModelBinding;
using XTI_Forms;

namespace XTI_WebApp.Extensions;

public sealed class FormModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }
        if (typeof(Form).IsAssignableFrom(context.Metadata.ModelType))
        {
            return new FormModelBinder();
        }
        return null;
    }
}