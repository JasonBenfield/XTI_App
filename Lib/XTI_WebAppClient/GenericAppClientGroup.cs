using XTI_App.Abstractions;

namespace XTI_WebAppClient;

public sealed class GenericAppClientGroup : AppClientGroup
{
    public GenericAppClientGroup(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, AppClientOptions options, string name)
        : base(httpClientFactory, xtiTokenAccessor, clientUrl, options, name)
    {
    }

    public Task ExecuteAction(string action, CancellationToken ct) =>
        CreatePostAction<EmptyRequest, EmptyActionResult>(action).Post("", new EmptyRequest(), ct);

    public Task<TResult> ExecuteAction<TResult>(string action, CancellationToken ct) =>
        CreatePostAction<EmptyRequest, TResult>(action).Post("", new EmptyRequest(), ct);

    public Task<TResult> ExecuteAction<TModel, TResult>(string action, TModel model, CancellationToken ct) =>
        CreatePostAction<TModel, TResult>(action).Post("", model, ct);

    public Task<TResult> ExecuteAction<TModel, TResult>(string action, string modifier, TModel model, CancellationToken ct) =>
        CreatePostAction<TModel, TResult>(action).Post(modifier, model, ct);
}