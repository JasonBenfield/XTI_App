using Microsoft.AspNetCore.OData.Query;

namespace XTI_ODataQuery.Api;

public interface QueryAction<TEntity>
{
    IQueryable<TEntity> Execute(ODataQueryOptions<TEntity> options);
}