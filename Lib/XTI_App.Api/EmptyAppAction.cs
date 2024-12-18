using XTI_App.Abstractions;

namespace XTI_App.Api;

public static class EmptyAppAction
{
    public static EmptyAppAction<TRequest, EmptyActionResult> Create<TRequest>() =>
        Create<TRequest, EmptyActionResult>();

    public static EmptyAppAction<TRequest, TResult> Create<TRequest, TResult>()
        where TResult : new() =>
        Create<TRequest, TResult>(() => new TResult());

    public static EmptyAppAction<TRequest, TResult> Create<TRequest, TResult>(Func<TResult> createDefault) =>
        new EmptyAppAction<TRequest, TResult>(createDefault);
}

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