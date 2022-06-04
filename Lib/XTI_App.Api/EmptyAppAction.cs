namespace XTI_App.Api;

public sealed class EmptyAppAction<TModel, TResult> : AppAction<TModel, TResult>
{
    private readonly Func<TResult> createDefault;

    public EmptyAppAction(Func<TResult> createDefault)
    {
        this.createDefault = createDefault;
    }

    public Task<TResult> Execute(TModel model, CancellationToken stoppingToken) => 
        Task.FromResult(createDefault());
}