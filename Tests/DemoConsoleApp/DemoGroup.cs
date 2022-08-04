using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class DemoGroup : AppApiGroupWrapper
{
    public DemoGroup(AppApiGroup source)
        : base(source)
    {
        First = source.AddAction
        (
            nameof(First),
            () => new FirstAction()
        );
        Second = source.AddAction
        (
            nameof(Second),
            () => new SecondAction()
        );
        Third = source.AddAction
        (
            nameof(Third),
            () => new ThirdAction()
        );
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> First { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Second { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Third { get; }
}