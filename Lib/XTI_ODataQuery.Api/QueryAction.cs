using Microsoft.AspNetCore.OData.Query;

namespace XTI_ODataQuery.Api;

public interface QueryAction<TArgs, TEntity>
{
    Task<IQueryable<TEntity>> Execute(ODataQueryOptions<TEntity> options, TArgs model);
}