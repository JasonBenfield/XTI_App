using XTI_App.Api;

namespace XTI_ConsoleApp;

public sealed class LifetimeGroup : AppApiGroupWrapper
{
    public LifetimeGroup(AppApiGroup source)
        : base(source)
    {
        StopApplication = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(StopApplication))
            .WithExecution<StopApplicationAction>()
            .Build();
    }

    public AppApiAction<EmptyRequest, EmptyActionResult> StopApplication { get; }
}