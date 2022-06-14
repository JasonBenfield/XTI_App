using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public class XtiODataController<TEntity> : ODataController
{
    private readonly IAppApiGroup groupApi;

    protected XtiODataController(IAppApiGroup groupApi)
    {
        this.groupApi = groupApi;
    }

    [HttpPost]
    [EnableQuery]
    public IQueryable<TEntity> Get(ODataQueryOptions<TEntity> model, CancellationToken ct) =>
        groupApi.Query<TEntity>(nameof(Get)).Execute(model, ct);

    public async Task<IActionResult> ToExcel(ODataQueryOptions<TEntity> model, CancellationToken ct)
    {
        var result = await groupApi.QueryToExcel<TEntity>(nameof(ToExcel)).Execute(model, ct);
        return File(result.FileStream, result.ContentType, result.DownloadName);
    }
}
