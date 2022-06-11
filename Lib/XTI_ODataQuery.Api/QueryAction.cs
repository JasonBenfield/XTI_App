using Microsoft.AspNetCore.OData.Query;

namespace XTI_ODataQuery.Api;

public interface QueryAction<TResult>
{
    IQueryable<TResult> Execute(ODataQueryOptions options);
}