using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Dynamic;
using System.Text.Json;
using XTI_Forms;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public sealed class FormModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }
        var model = (Form?)bindingContext.HttpContext.RequestServices.GetService(bindingContext.ModelType);
        if (model == null)
        {
            model = (Form?)Activator.CreateInstance(bindingContext.ModelType);
        }
        if (model == null) { throw new ArgumentNullException(nameof(model)); }
        var serialized = await new BodyFromRequest(bindingContext.HttpContext.Request).Serialize();
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonObjectConverter());
        var values = JsonSerializer.Deserialize<ExpandoObject>(serialized, options);
        if(values == null) { throw new ArgumentNullException(nameof(values)); }
        var dict = new Dictionary<string, object>();
        foreach(var kvp in values)
        {
            if(kvp.Value != null)
            {
                dict.Add(kvp.Key, kvp.Value);
            }
        }
        model.Import(dict);
        bindingContext.Result = ModelBindingResult.Success(model);
    }
}