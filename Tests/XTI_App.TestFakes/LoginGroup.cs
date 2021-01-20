using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class LoginGroup : AppApiGroupWrapper
    {
        public LoginGroup(AppApiGroup source) : base(source)
        {
            var factory = new AppApiActionFactory(source);
            Authenticate = source.AddAction
            (
                factory.Action
                (
                    nameof(Authenticate),
                    () => new EmptyAppAction<EmptyRequest, EmptyActionResult>()
                )
            );
        }
        public AppApiAction<EmptyRequest, EmptyActionResult> Authenticate { get; }
    }

}
