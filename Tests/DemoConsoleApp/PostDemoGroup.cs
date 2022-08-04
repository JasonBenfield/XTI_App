using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PostDemoGroup : AppApiGroupWrapper
{
    public PostDemoGroup(AppApiGroup source)
        : base(source)
    {
        First = source.AddAction
        (
            nameof(First),
            () => new PostFirstAction()
        );
        Second = source.AddAction
        (
            nameof(Second),
            () => new PostSecondAction()
        );
        Third = source.AddAction
        (
            nameof(Third),
            () => new PostThirdAction()
        );
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> First { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Second { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Third { get; }
}