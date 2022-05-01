using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PreDemoGroup : AppApiGroupWrapper
{
    public PreDemoGroup(AppApiGroup source)
        : base(source)
    {
        var actions = new AppApiActionFactory(source);
        First = source.AddAction
        (
            actions.Action
            (
                nameof(First),
                () => new PreFirstAction()
            )
        );
        Second = source.AddAction
        (
            actions.Action
            (
                nameof(Second),
                () => new PreSecondAction()
            )
        );
        Third = source.AddAction
        (
            actions.Action
            (
                nameof(Third),
                () => new PreThirdAction()
            )
        );
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> First { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Second { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Third { get; }
}