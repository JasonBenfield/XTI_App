namespace XTI_WebAppClient;

public sealed class AppClientGetAction<TModel>
{
    private readonly AppClientUrl clientUrl;
    private readonly string actionName;

    public AppClientGetAction(AppClientUrl clientUrl, string actionName)
    {
        this.clientUrl = clientUrl;
        this.actionName = actionName;
    }

    public Task<string> Url() => Url("");

    public Task<string> Url(string modifier) => _Url(default, modifier);

    public Task<string> Url(TModel model) => Url(model, "");

    public Task<string> Url(TModel model, string modifier) => _Url(model, modifier);

    private async Task<string> _Url(TModel? model, string modifier)
    {
        var url = await clientUrl.Value(actionName, modifier);
        var query = new ObjectToQueryString(model).Value;
        return $"{url}{query}";
    }
}
