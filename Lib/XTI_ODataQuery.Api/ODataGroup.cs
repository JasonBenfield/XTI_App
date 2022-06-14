using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public sealed class ODataGroup<TEntity> : AppApiGroupWrapper
{
    public ODataGroup
    (
        AppApiGroup source,
        Func<QueryAction<TEntity>> createQuery,
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

    public QueryApiAction<TEntity> Get { get; }
    public QueryToExcelApiAction<TEntity> ToExcel { get; }
}