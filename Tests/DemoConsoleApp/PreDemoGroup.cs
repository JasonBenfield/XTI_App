using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PreDemoGroup : AppApiGroupWrapper
{
    public PreDemoGroup(AppApiGroup source)
        : base(source)
    {
        First = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(First))
            .WithExecution(() => new PreFirstAction())
            .Build();
        Second = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(Second))
            .WithExecution(() => new PreSecondAction())
            .Build();
        Third = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(Third))
            .WithExecution(() => new PreThirdAction())
            .Build();
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> First { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Second { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Third { get; }
}