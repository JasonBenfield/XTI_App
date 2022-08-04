using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PreDemoGroup : AppApiGroupWrapper
{
    public PreDemoGroup(AppApiGroup source)
        : base(source)
    {
        First = source.AddAction
        (
            nameof(First),
            () => new PreFirstAction()
        );
        Second = source.AddAction
        (
            nameof(Second),
            () => new PreSecondAction()
        );
        Third = source.AddAction
        (
            nameof(Third),
            () => new PreThirdAction()
        );
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> First { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Second { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Third { get; }
}