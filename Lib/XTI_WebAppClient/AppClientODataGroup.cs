namespace XTI_WebAppClient;

public sealed class AppClientODataGroup<TArgs, TEntity> : AppClientGroup
{
    public AppClientODataGroup(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, AppClientOptions options, string name)
        : base(httpClientFactory, xtiTokenAccessor, clientUrl, options, name)
    {
        Actions = new AppClientODataGroupActions
        (
            Get: CreateODataAction<TArgs, TEntity>(nameof(Get)),
            ToExcel: CreateODataToExcelAction<TArgs, TEntity>(nameof(ToExcel))
        );
    }

    public AppClientODataGroupActions Actions { get; }

    public Task<ODataResult<TEntity>> Get(string modKey, string odataOptions, TArgs model) =>
        Actions.Get.Post(modKey, odataOptions, model);

    public Task<AppClientFileResult> ToExcel(string modKey, string odataOptions, TArgs model) =>
        Actions.ToExcel.GetFile(modKey, odataOptions, model);

    public sealed record AppClientODataGroupActions
    (
        AppClientODataAction<TArgs, TEntity> Get, 
        AppClientODataToExcelAction<TArgs, TEntity> ToExcel
    );
}