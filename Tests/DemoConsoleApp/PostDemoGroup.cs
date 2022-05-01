using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PostDemoGroup : AppApiGroupWrapper
{
    public PostDemoGroup(AppApiGroup source)
        : base(source)
    {
        var actions = new AppApiActionFactory(source);
        First = source.AddAction
        (
            actions.Action
            (
                nameof(First),
                () => new PostFirstAction()
            )
        );
        Second = source.AddAction
        (
            actions.Action
            (
                nameof(Second),
                () => new PostSecondAction()
            )
        );
        Third = source.AddAction
        (
            actions.Action
            (
                nameof(Third),
                () => new PostThirdAction()
            )
        );
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> First { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Second { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> Third { get; }
}