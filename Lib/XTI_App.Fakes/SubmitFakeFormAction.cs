using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class SubmitFakeFormAction : AppAction<FakeForm, string>
{
    public Task<string> Execute(FakeForm model, CancellationToken stoppingToken)
    {
        return Task.FromResult(model.TestText.Value() ?? "");
    }
}