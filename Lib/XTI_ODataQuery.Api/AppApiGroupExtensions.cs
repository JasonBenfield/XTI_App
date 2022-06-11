using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public static class AppApiGroupExtensions
{
    public static QueryApiAction<TEntity> AddQueryAction<TEntity>
    (
        this AppApiGroup group,
        string actionName,
        Func<QueryAction<TEntity>> createQuery,
        ResourceAccess? access = null,
        string friendlyName = ""
    ) =>
        (QueryApiAction<TEntity>)group.AddAction
        (
            actionName,
            friendlyName,
            (addData) => new QueryApiAction<TEntity>
            (
                addData.ActionPath,
                access ?? addData.GroupAccess,
                addData.User,
                createQuery,
                addData.FriendlyName
            )
        );

    public static QueryApiAction<TEntity> AddQueryToExcelAction<TEntity>
    (
        this AppApiGroup group,
        string actionName,
        Func<QueryAction<TEntity>> createQuery,
        Func<IQueryToExcel> createQueryToExcel,
        ResourceAccess? access = null,
        string friendlyName = ""
    ) =>
        (QueryApiAction<TEntity>)group.AddAction
        (
            actionName,
            friendlyName,
            (addData) => new QueryToExcelApiAction<TEntity>
            (
                addData.ActionPath,
                access ?? addData.GroupAccess,
                addData.User,
                createQuery,
                createQueryToExcel,
                addData.FriendlyName
            )
        );

    public static QueryApiAction<TEntity> Query<TEntity>(this IAppApiGroup group, string actionName) =>
        group.Action<QueryApiAction<TEntity>>(actionName);

    public static QueryToExcelApiAction<TEntity> QueryToExcel<TEntity>(this IAppApiGroup group, string actionName) =>
        group.Action<QueryToExcelApiAction<TEntity>>(actionName);

}
