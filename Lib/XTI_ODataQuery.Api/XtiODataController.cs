using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public class XtiODataController<TArgs, TEntity> : ODataController
{
    private readonly IAppApiGroup groupApi;

    protected XtiODataController(IAppApiGroup groupApi)
    {
        this.groupApi = groupApi;
    }

    [HttpPost]
    [EnableQuery]
    public Task<IQueryable<TEntity>> Get(ODataQueryOptions<TEntity> odataQuery, TArgs model, CancellationToken ct)=>
        groupApi.Query<TArgs, TEntity>(nameof(Get)).Execute(odataQuery, model, ct);

    [Route("~/[controller]/ToExcel")]
    [HttpGet]
    public async Task<IActionResult> ToExcel(ODataQueryOptions<TEntity> odataQuery, TArgs model, CancellationToken ct)
    {
        var result = await groupApi.QueryToExcel<TArgs, TEntity>(nameof(ToExcel)).Execute(odataQuery, model, ct);
        return File(result.FileStream, result.ContentType, result.DownloadName);
    }
}
