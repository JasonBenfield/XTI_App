using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class AddProductAction : AppAction<AddProductModel, int>
{
    public Task<int> Execute(AddProductModel model, CancellationToken stoppingToken)
    {
        return Task.FromResult(1);
    }
}