using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public static class AppApiGroupExtensions
{
    public static QueryApiActionBuilder<TArgs, TEntity> AddQueryAction<TArgs, TEntity>(this AppApiGroup group, string actionName) =>
        (QueryApiActionBuilder<TArgs, TEntity>)group.AddAction
        (
            actionName,
            (addData) => new QueryApiActionBuilder<TArgs, TEntity>
            (
                addData.Services,
                group.Path,
                addData.User,
                addData.GroupAccessBuilder,
                actionName
            )
        );

    public static QueryApiAction<TArgs, TEntity> Query<TArgs, TEntity>(this IAppApiGroup group, string actionName) =>
        group.Action<QueryApiAction<TArgs, TEntity>>(actionName);

    public static QueryToExcelApiActionBuilder<TArgs, TEntity> AddQueryToExcelAction<TArgs, TEntity>(this AppApiGroup group, string actionName) =>
        (QueryToExcelApiActionBuilder<TArgs, TEntity>)group.AddAction
        (
            actionName,
            (addData) => new QueryToExcelApiActionBuilder<TArgs, TEntity>
            (
                addData.Services,
                group.Path,
                addData.User,
                addData.GroupAccessBuilder,
                actionName
            )
        );

    public static QueryToExcelApiAction<TArgs, TEntity> QueryToExcel<TArgs, TEntity>(this IAppApiGroup group, string actionName) =>
        group.Action<QueryToExcelApiAction<TArgs, TEntity>>(actionName);

}
