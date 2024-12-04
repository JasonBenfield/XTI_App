using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class HomeGroup : AppApiGroupWrapper
{
    public HomeGroup(AppApiGroup source)
        : base(source)
    {

        DoSomething = source.AddAction<SampleRequestData, SampleResultData>()
            .Named(nameof(DoSomething))
            .WithExecution(() => new EmptyAppAction<SampleRequestData, SampleResultData>(() => new SampleResultData("TEST01", 1)))
            .Build();
        LogRequestData = source.AddAction<SampleRequestData, EmptyActionResult>()
            .Named(nameof(LogRequestData))
            .WithExecution<LogRequestDataAction>()
            .AlwaysLogRequestData()
            .Build();
        LogRequestDataOnError = source.AddAction<SampleRequestData, EmptyActionResult>()
            .Named(nameof(LogRequestDataOnError))
            .WithExecution<LogRequestDataOnErrorAction>()
            .LogRequestDataOnError()
            .Build();
        LogResultData = source.AddAction<SampleRequestData, SampleResultData>()
            .Named(nameof(LogResultData))
            .WithExecution<LogResultDataAction>()
            .LogResultData()
            .Build();
    }
    public AppApiAction<SampleRequestData, SampleResultData> DoSomething { get; }
    public AppApiAction<SampleRequestData, EmptyActionResult> LogRequestData { get; }
    public AppApiAction<SampleRequestData, EmptyActionResult> LogRequestDataOnError { get; }
    public AppApiAction<SampleRequestData, SampleResultData> LogResultData { get; }
}