using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class GetInfoAction : AppAction<EmptyRequest, string>
{
    public Task<string> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        return Task.FromResult("");
    }
}