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

    public Task<ODataResult<TEntity>> Get(string modKey, string odataOptions, TArgs requestData, CancellationToken ct) =>
        Actions.Get.Post(modKey, odataOptions, requestData, ct);

    public Task<AppClientFileResult> ToExcel(string modKey, string odataOptions, TArgs requestData, CancellationToken ct) =>
        Actions.ToExcel.GetFile(modKey, odataOptions, requestData, ct);

    public sealed record AppClientODataGroupActions
    (
        AppClientODataAction<TArgs, TEntity> Get,
        AppClientODataToExcelAction<TArgs, TEntity> ToExcel
    );
}