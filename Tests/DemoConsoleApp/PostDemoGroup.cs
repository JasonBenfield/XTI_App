using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PostDemoGroup : AppApiGroupWrapper
{
    public PostDemoGroup(AppApiGroup source)
        : base(source)
    {
        First = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(First))
            .WithExecution(() => new PostFirstAction())
            .Build();
        Second = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(Second))
            .WithExecution(() => new PostSecondAction())
            .Build();
        Third = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(Third))
            .WithExecution(() => new PostThirdAction())
            .Build();
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> First { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Second { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Third { get; }
}