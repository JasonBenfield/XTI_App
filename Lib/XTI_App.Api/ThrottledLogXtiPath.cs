using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Api;

public sealed class ThrottledLogXtiPath
{
    private readonly XtiPath xtiPath;
    private readonly TimeSpan requestInterval;
    private readonly TimeSpan exceptionInterval;

    public ThrottledLogXtiPath(XtiPath xtiPath, TimeSpan requestInterval, TimeSpan exceptionInterval)
    {
        this.xtiPath = xtiPath;
        this.requestInterval = requestInterval;
        this.exceptionInterval = exceptionInterval;
    }

    public ThrottledLogPath Value(XtiBasePath xtiBasePath) =>
        new
        (
            path: xtiBasePath.Finish(xtiPath.Group, xtiPath.Action).Value(),
            throttleRequestInterval: requestInterval,
            throttleExceptionInterval: exceptionInterval
        );
}
