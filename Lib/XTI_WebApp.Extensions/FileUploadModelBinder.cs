using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.ObjectModel;
using System.Diagnostics;
using XTI_App.Abstractions;

namespace XTI_WebApp.Extensions;

internal sealed partial class FileUploadModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);
        var postedFiles = new List<FileUpload>();
        // If we're at the top level, then use the FieldName (parameter or property name).
        // This handles the fact that there will be nothing in the ValueProviders for this parameter
        // and so we'll do the right thing even though we 'fell-back' to the empty prefix.
        var modelName = bindingContext.IsTopLevelObject
            ? bindingContext.BinderModelName ?? bindingContext.FieldName
            : bindingContext.ModelName;

        await GetFormFilesAsync(modelName, bindingContext, postedFiles);

        // If ParameterBinder incorrectly overrode ModelName, fall back to OriginalModelName prefix. Comparisons
        // are tedious because e.g. top-level parameter or property is named Blah and it contains a BlahBlah
        // property. OriginalModelName may be null in tests.
        if (postedFiles.Count == 0 &&
            bindingContext.OriginalModelName != null &&
            !string.Equals(modelName, bindingContext.OriginalModelName, StringComparison.Ordinal) &&
            !modelName.StartsWith(bindingContext.OriginalModelName + "[", StringComparison.Ordinal) &&
            !modelName.StartsWith(bindingContext.OriginalModelName + ".", StringComparison.Ordinal))
        {
            modelName = ModelNames.CreatePropertyModelName(bindingContext.OriginalModelName, modelName);
            await GetFormFilesAsync(modelName, bindingContext, postedFiles);
        }
        object value;
        if (bindingContext.ModelType == typeof(FileUpload))
        {
            if (postedFiles.Count == 0)
            {
                return;
            }
            value = postedFiles.First();
        }
        else
        {
            if (postedFiles.Count == 0 && !bindingContext.IsTopLevelObject)
            {
                return;
            }
            var modelType = bindingContext.ModelType;
            if (modelType == typeof(FileUpload[]))
            {
                Debug.Assert(postedFiles is List<FileUpload>);
                value = postedFiles;
            }
            else
            {
                value = postedFiles;
            }
        }
        // We need to add a ValidationState entry because the modelName might be non-standard. Otherwise
        // the entry we create in model state might not be marked as valid.
        bindingContext.ValidationState.Add
        (
            value, 
            new ValidationStateEntry
            {
                Key = modelName,
            }
        );
        bindingContext.ModelState.SetModelValue
        (
            modelName,
            rawValue: null,
            attemptedValue: null
        );
        bindingContext.Result = ModelBindingResult.Success(value);
    }

    private async Task GetFormFilesAsync
    (
        string modelName,
        ModelBindingContext bindingContext,
        ICollection<FileUpload> postedFiles
    )
    {
        var request = bindingContext.HttpContext.Request;
        if (request.HasFormContentType)
        {
            var form = await request.ReadFormAsync();

            foreach (var file in form.Files)
            {
                // If there is an <input type="file" ... /> in the form and is left blank.
                if (file.Length == 0 && string.IsNullOrEmpty(file.FileName))
                {
                    continue;
                }
                if (file.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase))
                {
                    var fileUpload = new FileUpload
                    (
                        file.OpenReadStream(),
                        file.ContentType,
                        file.FileName
                    );
                    postedFiles.Add(fileUpload);
                }
            }
        }
    }
}
