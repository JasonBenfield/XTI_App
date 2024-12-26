using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_WebApp.Extensions;

public sealed class VerifyModelStateActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid && context.HttpContext.Request.HasJsonContentType())
        {
            throw new ValidationFailedException
            (
                context.ModelState.Values
                    .Where(v => v.ValidationState == ModelValidationState.Invalid)
                    .SelectMany(v => v.Errors)
                    .Select(err => new ErrorModel(err.ErrorMessage))
                    .ToArray()
            );
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

}
