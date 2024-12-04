using XTI_App.Api;

namespace XTI_App.Fakes;

internal sealed class LogRequestDataAction : AppAction<SampleRequestData, EmptyActionResult>
{
    private readonly FakeError fakeError;

    public LogRequestDataAction(FakeError fakeError)
    {
        this.fakeError = fakeError;
    }

    public Task<EmptyActionResult> Execute(SampleRequestData model, CancellationToken stoppingToken)
    {
        fakeError.ThrowIfRequired();
        return Task.FromResult(new EmptyActionResult());
    }
}
