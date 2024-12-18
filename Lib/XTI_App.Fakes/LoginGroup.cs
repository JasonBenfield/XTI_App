using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class LoginGroup : AppApiGroupWrapper
{
    public LoginGroup(AppApiGroup source) : base(source)
    {
        Authenticate = source.AddAction<EmptyRequest, EmptyActionResult>()
            .Named(nameof(Authenticate))
            .WithExecution(() => new EmptyAppAction<EmptyRequest, EmptyActionResult>(() => new EmptyActionResult()))
            .Build();
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> Authenticate { get; }
}