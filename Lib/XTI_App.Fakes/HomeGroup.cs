using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class HomeGroup : AppApiGroupWrapper
{
    public HomeGroup(AppApiGroup source)
        : base(source)
    {

        DoSomething = source.AddAction
        (
            nameof(DoSomething),
            () => new EmptyAppAction<EmptyRequest, EmptyActionResult>(() => new EmptyActionResult())
        );
    }
    public AppApiAction<EmptyRequest, EmptyActionResult> DoSomething { get; }
}