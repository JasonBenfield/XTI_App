using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class DemoGroup : AppApiGroupWrapper
{
    public DemoGroup(AppApiGroup source)
        : base(source)
    {
        var actions = new AppApiActionFactory(source);
        First = source.AddAction
        (
            actions.Action
            (
                nameof(First),
                () => new FirstAction()
            )
        );
        Second = source.AddAction
        (
            actions.Action
            (
                nameof(Second),
                () => new SecondAction()
            )
        );
        Third = source.AddAction
        (
            actions.Action
            (
                nameof(Third),
                () => new ThirdAction()
            )
        );
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> First { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Second { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Third { get; }
}