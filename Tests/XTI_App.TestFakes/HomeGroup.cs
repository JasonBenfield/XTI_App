using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class HomeGroup : AppApiGroupWrapper
    {
        public HomeGroup(AppApiGroup source)
            : base(source)
        {
            var actions = new AppApiActionFactory(source);
            DoSomething = source.AddAction
            (
                actions.Action
                (
                    nameof(DoSomething),
                    () => new EmptyAppAction<EmptyRequest, EmptyActionResult>()
                )
            );
        }
        public AppApiAction<EmptyRequest, EmptyActionResult> DoSomething { get; }
    }

}
