namespace XTI_WebAppClient;

public class AppClientGroup
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;
    private readonly AppClientOptions options;

    protected AppClientGroup(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, AppClientOptions options, string name)
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl.WithGroup(name);
        this.options = options;
    }

    protected AppClientPostAction<TModel, TResult> CreatePostAction<TModel, TResult>(string actionName) =>
        new AppClientPostAction<TModel, TResult>(httpClientFactory, xtiTokenAccessor, clientUrl, options, actionName);

    protected AppClientContentAction<TModel> CreateContentAction<TModel>(string actionName) =>
        new AppClientContentAction<TModel>(httpClientFactory, xtiTokenAccessor, clientUrl, options, actionName);

    protected AppClientGetAction<TModel> CreateGetAction<TModel>(string actionName) =>
        new AppClientGetAction<TModel>(httpClientFactory, xtiTokenAccessor, clientUrl, options, actionName);

    protected AppClientFileAction<TModel> CreateFileAction<TModel>(string actionName) =>
        new AppClientFileAction<TModel>(httpClientFactory, xtiTokenAccessor, clientUrl, options, actionName);

    protected AppClientODataAction<TModel, TEntity> CreateODataAction<TModel, TEntity>(string actionName) =>
        new AppClientODataAction<TModel, TEntity>(httpClientFactory, xtiTokenAccessor, clientUrl, options, actionName);

    protected AppClientODataToExcelAction<TModel, TEntity> CreateODataToExcelAction<TModel, TEntity>(string actionName) =>
        new AppClientODataToExcelAction<TModel, TEntity>(httpClientFactory, xtiTokenAccessor, clientUrl, options, actionName);

}