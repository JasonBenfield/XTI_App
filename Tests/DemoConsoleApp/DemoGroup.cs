using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class DemoGroup : AppApiGroupWrapper
{
    public DemoGroup(AppApiGroup source)
        : base(source)
    {
        First = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(First))
            .WithExecution(() => new FirstAction())
            .Build();
        Second = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(Second))
            .WithExecution(() => new SecondAction())
            .Build();
        Third = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(Third))
            .WithExecution(() => new ThirdAction())
            .Build();
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> First { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Second { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Third { get; }
}