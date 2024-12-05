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
        Get = source.AddQueryAction<TArgs, TEntity>(nameof(Get))
            .WithQuery(createQuery)
            .WithAccess(access ?? source.Access)
            .Build();
        ToExcel = source.AddQueryToExcelAction<TArgs, TEntity>(nameof(ToExcel))
            .WithQuery(createQuery)
            .WithAccess(access ?? source.Access)
            .Build();
    }

    public QueryApiAction<TArgs, TEntity> Get { get; }
    public QueryToExcelApiAction<TArgs, TEntity> ToExcel { get; }
}