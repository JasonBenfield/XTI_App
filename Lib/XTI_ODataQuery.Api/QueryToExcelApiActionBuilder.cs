﻿using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public sealed class QueryToExcelApiActionBuilder<TArgs, TEntity> : IAppApiActionBuilder
{
    private readonly IServiceProvider sp;
    private readonly XtiPath groupPath;
    private readonly IAppApiUser user;
    private string friendlyName = "";
    private readonly ActionThrottledLogPathBuilder<QueryToExcelApiActionBuilder<TArgs, TEntity>> throttledLogPathBuilder;
    private ResourceAccessBuilder accessBuilder;
    private Func<QueryAction<TArgs, TEntity>> createQuery;
    private Func<IQueryToExcel> createQueryToExcel;
    private QueryToExcelApiAction<TArgs, TEntity>? builtAction;

    internal QueryToExcelApiActionBuilder
    (
        IServiceProvider sp,
        XtiPath groupPath,
        IAppApiUser user,
        ResourceAccessBuilder groupAccessBuilder,
        string actionName
    )
    {
        this.sp = sp;
        this.groupPath = groupPath;
        this.user = user;
        accessBuilder = new ResourceAccessBuilder(groupAccessBuilder);
        throttledLogPathBuilder = new(this);
        ActionName = actionName;
        createQuery = () => new EmptyQueryAction<TArgs, TEntity>();
        createQueryToExcel = () => new DefaultQueryToExcelBuilder().Build();
    }

    public string ActionName { get; }

    public QueryToExcelApiActionBuilder<TArgs, TEntity> WithFriendlyName(string friendlyName)
    {
        this.friendlyName = friendlyName;
        return this;
    }

    public QueryToExcelApiActionBuilder<TArgs, TEntity> AllowAnonymousAccess()
    {
        accessBuilder.AllowAnonymous();
        return this;
    }

    public QueryToExcelApiActionBuilder<TArgs, TEntity> ResetAccess()
    {
        accessBuilder.Reset();
        return this;
    }

    public QueryToExcelApiActionBuilder<TArgs, TEntity> ResetAccess(ResourceAccess access)
    {
        accessBuilder.Reset(access);
        return this;
    }

    public QueryToExcelApiActionBuilder<TArgs, TEntity> ResetAccessWithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.Reset(roleNames);
        return this;
    }

    public QueryToExcelApiActionBuilder<TArgs, TEntity> WithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithAllowed(roleNames);
        return this;
    }

    public QueryToExcelApiActionBuilder<TArgs, TEntity> WithoutAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithoutAllowed(roleNames);
        return this;
    }

    public QueryToExcelApiActionBuilder<TArgs, TEntity> WithQuery<TExecution>() where TExecution : QueryAction<TArgs, TEntity>
    {
        WithQuery(() => sp.GetRequiredService<TExecution>());
        return this;
    }

    public QueryToExcelApiActionBuilder<TArgs, TEntity> WithQuery(Func<QueryAction<TArgs, TEntity>> createQuery)
    {
        this.createQuery = createQuery;
        return this;
    }

    public QueryToExcelApiActionBuilder<TArgs, TEntity> WithQueryToExcel<TQueryToExcel>() where TQueryToExcel : IQueryToExcel
    {
        WithQueryToExcel(() => sp.GetRequiredService<TQueryToExcel>());
        return this;
    }

    public QueryToExcelApiActionBuilder<TArgs, TEntity> WithQueryToExcel(Func<IQueryToExcel> createQueryToExcel)
    {
        this.createQueryToExcel = createQueryToExcel;
        return this;
    }

    public ActionThrottledLogIntervalBuilder<QueryToExcelApiActionBuilder<TArgs, TEntity>> ThrottleRequestLogging() =>
        throttledLogPathBuilder.RequestIntervalBuilder;

    public ActionThrottledLogIntervalBuilder<QueryToExcelApiActionBuilder<TArgs, TEntity>> ThrottleExceptionLogging() =>
        throttledLogPathBuilder.ExceptionIntervalBuilder;

    public XtiPath ActionPath() => groupPath.WithAction(ActionName);

    public QueryToExcelApiAction<TArgs, TEntity> Build()
    {
        builtAction ??= BuildAction();
        return builtAction;
    }

    private QueryToExcelApiAction<TArgs, TEntity> BuildAction()
    {
        var actionPath = ActionPath();
        var scheduledBuilder = new ActionScheduleBuilder<QueryToExcelApiActionBuilder<TArgs, TEntity>>(this);
        return new QueryToExcelApiAction<TArgs, TEntity>
        (
            actionPath,
            accessBuilder.Build(),
            user,
            createQuery,
            createQueryToExcel,
            new AppActionFriendlyName(friendlyName, ActionName).Value,
            throttledLogPathBuilder.Build(actionPath),
            scheduledBuilder.Build()
        );
    }

    IAppApiAction IAppApiActionBuilder.Build() => Build();
}
