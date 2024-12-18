using Microsoft.AspNetCore.OData.Query;

namespace XTI_ODataQuery.Api;

public sealed class EmptyQueryAction<TArgs, TEntity> : QueryAction<TArgs, TEntity>
{
    public Task<IQueryable<TEntity>> Execute(ODataQueryOptions<TEntity> options, TArgs requestData) =>
        Task.FromResult(new TEntity[0].AsQueryable());
}