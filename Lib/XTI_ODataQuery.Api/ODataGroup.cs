using Microsoft.AspNetCore.Http;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public sealed class ODataGroup<TArgs, TEntity> : AppApiGroupWrapper
{
    public ODataGroup
    (
        AppApiGroup source,
        Func<QueryAction<TArgs, TEntity>> createQuery,
        Func<DefaultQueryToExcelBuilder>? createQueryToExcelBuilder = null,
        ResourceAccess? access = null
    )
        : base(source)
    {
        Get = source.AddQueryAction
        (
            nameof(Get),
            createQuery,
            access
        );
        ToExcel = source.AddQueryToExcelAction
        (
            nameof(ToExcel),
            createQuery,
            createQueryToExcelBuilder ?? (() => new DefaultQueryToExcelBuilder()),
            access
        );
    }

    public QueryApiAction<TArgs, TEntity> Get { get; }
    public QueryToExcelApiAction<TArgs, TEntity> ToExcel { get; }
}