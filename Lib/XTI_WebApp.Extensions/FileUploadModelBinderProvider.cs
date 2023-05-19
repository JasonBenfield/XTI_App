using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using XTI_App.Abstractions;

namespace XTI_WebApp.Extensions;

internal sealed class FileUploadModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }
        if (context.Metadata.ModelType == typeof(FileUpload) || context.Metadata.ModelType == typeof(FileUpload[]))
        {
            return new BinderTypeModelBinder(typeof(FileUploadModelBinder));
        }
        return null;
    }
}
