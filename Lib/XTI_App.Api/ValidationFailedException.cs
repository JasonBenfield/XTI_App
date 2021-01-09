using System.Collections.Generic;
using System.Linq;
using XTI_Core;

namespace XTI_App.Api
{
    public sealed class ValidationFailedException : AppException
    {
        public ValidationFailedException(IEnumerable<ErrorModel> errors)
            : base
            (
                "Validation failed with the following errors:\r\n"
                + string.Join("\r\n", errors.Select(e => e.Message))
            )
        {
            Errors = errors;
        }

        public IEnumerable<ErrorModel> Errors { get; }
    }
}
