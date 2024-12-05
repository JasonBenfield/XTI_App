using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class ValidationFailedException : AppException
{
    public ValidationFailedException(params string[] errors)
        : this(errors.Select(e => new ErrorModel(e)).ToArray(), null)
    {
    }

    public ValidationFailedException(params ErrorModel[] errors)
        : this(errors, null)
    {
    }

    public ValidationFailedException(ErrorModel[] errors, Exception? innerException)
        : base
        (
            "Validation failed with the following errors:\r\n"
            + string.Join("\r\n", errors.Select(e => e.Message)),
            innerException
        )
    {
        Errors = errors;
    }

    public ErrorModel[] Errors { get; }
}