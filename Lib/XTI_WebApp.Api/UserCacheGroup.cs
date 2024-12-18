using Microsoft.Extensions.DependencyInjection;
using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class UserCacheGroup : AppApiGroupWrapper
{
    public UserCacheGroup(AppApiGroup source) : base(source)
    {
        ClearCache = source.AddAction<string, EmptyActionResult>()
            .Named(nameof(ClearCache))
            .WithExecution<ClearCacheAction>()
            .ThrottleRequestLogging().ForOneHour()
            .ThrottleExceptionLogging().For(5).Minutes()
            .Build();
    }

    public AppApiAction<string, EmptyActionResult> ClearCache { get; }
}