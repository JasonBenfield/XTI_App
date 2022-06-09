namespace XTI_WebAppClient;

public sealed class GenericAppClientGroup : AppClientGroup
{
    public GenericAppClientGroup(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, string name)
        : base(httpClientFactory, xtiTokenAccessor, clientUrl, name)
    {
    }

    public Task ExecuteAction(string action) =>
        Post<EmptyActionResult, EmptyRequest>(action, "", new EmptyRequest());

    public Task<TResult> ExecuteAction<TResult>(string action) =>
        Post<TResult, EmptyRequest>(action, "", new EmptyRequest());

    public Task<TResult> ExecuteAction<TResult, TModel>(string action, TModel model) =>
        Post<TResult, TModel>(action, "", model);

    public Task<TResult> ExecuteAction<TResult, TModel>(string action, string modifier, TModel model) =>
        Post<TResult, TModel>(action, modifier, model);
}