using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public sealed class ODataGroup<TArgs, TEntity> : AppApiGroupWrapper
{
    internal ODataGroup
    (
        AppApiGroup source,
        ODataGroupBuilder<TArgs, TEntity> builder
    )
        : base(source)
    {
        Get = builder.Get.Build();
        ToExcel = builder.ToExcel.Build();
    }

    public QueryApiAction<TArgs, TEntity> Get { get; }
    public QueryToExcelApiAction<TArgs, TEntity> ToExcel { get; }
}