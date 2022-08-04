using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public static class AppApiGroupExtensions
{
    public static QueryApiAction<TArgs, TEntity> AddQueryAction<TArgs, TEntity>
    (
        this AppApiGroup group,
        string actionName,
        Func<QueryAction<TArgs, TEntity>> createQuery,
        ResourceAccess? access = null,
        string friendlyName = ""
    ) =>
        (QueryApiAction<TArgs, TEntity>)group.AddAction
        (
            actionName,
            friendlyName,
            (addData) => new QueryApiAction<TArgs, TEntity>
            (
                addData.ActionPath,
                access ?? addData.GroupAccess,
                addData.User,
                createQuery,
                addData.FriendlyName
            )
        );

    public static QueryApiAction<TArgs, TEntity> Query<TArgs, TEntity>(this IAppApiGroup group, string actionName) =>
        group.Action<QueryApiAction<TArgs, TEntity>>(actionName);

    public static QueryToExcelApiAction<TArgs, TEntity> AddQueryToExcelAction<TArgs, TEntity>
    (
        this AppApiGroup group,
        string actionName,
        Func<QueryAction<TArgs, TEntity>> createQuery,
        Func<DefaultQueryToExcelBuilder>? createQueryToExcelBuilder = null,
        ResourceAccess? access = null,
        string friendlyName = ""
    ) =>
        group.AddQueryToExcelAction
        (
            actionName,
            createQuery,
            () => (createQueryToExcelBuilder?.Invoke() ?? new DefaultQueryToExcelBuilder()).Build(),
            access,
            friendlyName
        );

    public static QueryToExcelApiAction<TArgs, TEntity> AddQueryToExcelAction<TArgs, TEntity>
    (
        this AppApiGroup group,
        string actionName,
        Func<QueryAction<TArgs, TEntity>> createQuery,
        Func<IQueryToExcel> createQueryToExcel,
        ResourceAccess? access = null,
        string friendlyName = ""
    ) =>
        (QueryToExcelApiAction<TArgs, TEntity>)group.AddAction
        (
            actionName,
            friendlyName,
            (addData) => new QueryToExcelApiAction<TArgs, TEntity>
            (
                addData.ActionPath,
                access ?? addData.GroupAccess,
                addData.User,
                createQuery,
                createQueryToExcel,
                addData.FriendlyName
            )
        );

    public static QueryToExcelApiAction<TArgs, TEntity> QueryToExcel<TArgs, TEntity>(this IAppApiGroup group, string actionName) =>
        group.Action<QueryToExcelApiAction<TArgs, TEntity>>(actionName);

}
