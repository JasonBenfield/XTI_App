using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.UriParser;
using System.Reflection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_TempLog;
using XTI_WebApp.Api;

namespace XTI_ODataQuery.Api;

public sealed class QueryToExcelApiAction<TArgs, TEntity> : IAppApiAction
{
    private readonly IAppApiUser user;
    private readonly Func<QueryAction<TArgs, TEntity>> createQuery;
    private readonly Func<IQueryToExcel> createQueryToExcel;
    private readonly ThrottledLogXtiPath throttledLogPath;

    public QueryToExcelApiAction
    (
        XtiPath path,
        ResourceAccess access,
        IAppApiUser user,
        Func<QueryAction<TArgs, TEntity>> createQuery,
        Func<IQueryToExcel> createQueryToExcel,
        string friendlyName,
        ThrottledLogXtiPath throttledLogPath
    )
    {
        Path = path;
        Access = access;
        this.user = user;
        FriendlyName = friendlyName;
        this.createQueryToExcel = createQueryToExcel;
        this.createQuery = createQuery;
        this.throttledLogPath = throttledLogPath;
    }

    public XtiPath Path { get; }
    public string ActionName { get => Path.Action.DisplayText.Replace(" ", ""); }
    public ResourceAccess Access { get; }
    public string FriendlyName { get; }

    public ThrottledLogPath ThrottledLogPath(XtiBasePath xtiBasePath) => throttledLogPath.Value(xtiBasePath);

    public async Task<WebFileResult> Execute(ODataQueryOptions<TEntity> options, TArgs model, CancellationToken stoppingToken = default)
    {
        await user.EnsureUserHasAccess(Path);
        var queryAction = createQuery();
        var result = await queryAction.Execute(options, model);
        if(options.Filter != null)
        {
            result = options.Filter.ApplyTo(result, new ODataQuerySettings()) as IQueryable<TEntity>;
        }
        if(options.OrderBy != null)
        {
            result = options.OrderBy.ApplyTo(result, new ODataQuerySettings());
        }
        var queryRecords = result!.ToArray();
        var selectFields = options.SelectExpand.SelectExpandClause.SelectedItems
            .OfType<PathSelectItem>()
            .Select(item => item.SelectedPath)
            .OfType<ODataSelectPath>()
            .Select(item => item.FirstSegment.Identifier)
            .ToArray();
        var properties = queryRecords
            .FirstOrDefault()?.GetType().GetProperties()
            ?? [];
        if (!selectFields.Any())
        {
            selectFields = properties.Select(p => p.Name).ToArray();
        }
        var queryDictionaryRecords = queryRecords
            .Select
            (
                obj => 
                    properties
                        .ToDictionary
                        (
                            p => p.Name, 
                            p => p.GetValue(obj) ?? new EmptyObject()
                        )
            )
            .ToArray();
        var queryResult = new QueryResult
        (
            new RawQueryValues(options.RawValues),
            selectFields,
            queryDictionaryRecords
        );
        var queryToExcel = createQueryToExcel();
        var excelStream = await queryToExcel.ToExcel(queryResult, stoppingToken);
        if (excelStream.CanSeek)
        {
            excelStream.Seek(0, SeekOrigin.Begin);
        }
        return new WebFileResult
        (
            excelStream,
            WebContentTypes.Excel,
            queryToExcel.DownloadName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)
                ? queryToExcel.DownloadName
                : $"{queryToExcel.DownloadName}_{DateTime.Now:yyMMdd_HHmmss}.xlsx"
        );
    }

    public AppApiActionTemplate Template()
    {
        var modelTemplate = new ValueTemplateFromType(typeof(TArgs)).Template();
        var resultTemplate = new EmptyValueTemplate();
        return new AppApiActionTemplate
        (
            Path.Action.DisplayText,
            FriendlyName,
            Access,
            modelTemplate,
            resultTemplate,
            ResourceResultType.Values.QueryToExcel
        );
    }
}
