﻿using XTI_App.Api;

namespace XTI_ODataQuery.Api;

public sealed class ODataGroupBuilder<TArgs, TEntity>
{
    private readonly AppApiGroup source;

    public ODataGroupBuilder(AppApiGroup source)
    {
        this.source = source;
        Get = source.AddQueryAction<TArgs, TEntity>(nameof(Get));
        ToExcel = source.AddQueryToExcelAction<TArgs, TEntity>(nameof(ToExcel));
    }

    public ODataGroupBuilder<TArgs, TEntity> WithQuery<TExecution>() where TExecution : QueryAction<TArgs, TEntity>
    {
        Get.WithQuery<TExecution>();
        ToExcel.WithQuery<TExecution>();
        return this;
    }

    public ODataGroupBuilder<TArgs, TEntity> WithQuery(Func<QueryAction<TArgs, TEntity>> createQuery)
    {
        Get.WithQuery(createQuery);
        ToExcel.WithQuery(createQuery);
        return this;
    }

    public ODataGroupBuilder<TArgs, TEntity> WithQueryToExcel<TQueryToExcel>() where TQueryToExcel : IQueryToExcel
    {
        ToExcel.WithQueryToExcel<TQueryToExcel>();
        return this;
    }

    public ODataGroupBuilder<TArgs, TEntity> WithQueryToExcel(Func<IQueryToExcel> createQueryToExcel)
    {
        ToExcel.WithQueryToExcel(createQueryToExcel);
        return this;
    }

    public QueryApiActionBuilder<TArgs, TEntity> Get { get; }
    public QueryToExcelApiActionBuilder<TArgs, TEntity> ToExcel { get; }

    public ODataGroup<TArgs, TEntity> Build() =>
        new ODataGroup<TArgs, TEntity>(source, this);
}