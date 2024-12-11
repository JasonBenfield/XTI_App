using Microsoft.Extensions.DependencyInjection;
using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class UserCacheGroup : AppApiGroupWrapper
{
    public UserCacheGroup(AppApiGroup source, IServiceProvider sp) : base(source)
    {
        ClearCache = source.AddAction<string, EmptyActionResult>()
            .Named(nameof(ClearCache))
            .WithExecution
            (
                () => new ClearCacheAction
                (
                    sp.GetRequiredService<ICachedUserContext>()
                )
            )
            .Build();
    }

    public AppApiAction<string, EmptyActionResult> ClearCache { get; }
}