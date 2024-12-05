using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class LogResultDataAction : AppAction<SampleRequestData, SampleResultData>
{
    private readonly FakeError fakeError;

    public static readonly SampleResultData Result = new SampleResultData("TEST01", 1);

    public LogResultDataAction(FakeError fakeError)
    {
        this.fakeError = fakeError;
    }

    public Task<SampleResultData> Execute(SampleRequestData model, CancellationToken stoppingToken)
    {
        fakeError.ThrowIfRequired();
        return Task.FromResult(Result);
    }
}
