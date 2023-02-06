using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class LoginGroup : AppApiGroupWrapper
{
    public LoginGroup(AppApiGroup source) : base(source)
    {
        Authenticate = source.AddAction
        (
            nameof(Authenticate),
            () => new EmptyAppAction<EmptyRequest, EmptyActionResult>(() => new EmptyActionResult())
        );
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> Authenticate { get; }
}