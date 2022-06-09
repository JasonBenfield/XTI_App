using Microsoft.Extensions.DependencyInjection;
using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class UserCacheGroup : AppApiGroupWrapper
{
    public UserCacheGroup(AppApiGroup source, IServiceProvider sp) : base(source)
    {
        var actions = new WebAppApiActionFactory(source);
        ClearCache = source.AddAction
        (
            actions.Action
            (
                nameof(ClearCache),
                () => new ClearCacheAction
                (
                    sp.GetRequiredService<ICachedUserContext>()
                )
            )
        );
    }

    public AppApiAction<string, EmptyActionResult> ClearCache { get; }
}