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

    public Task<TResult> ExecuteAction<TRequest, TResult>(string action, TRequest requestData, CancellationToken ct) =>
        CreatePostAction<TRequest, TResult>(action).Post("", requestData, ct);

    public Task<TResult> ExecuteAction<TRequest, TResult>(string action, string modifier, TRequest requestData, CancellationToken ct) =>
        CreatePostAction<TRequest, TResult>(action).Post(modifier, requestData, ct);
}