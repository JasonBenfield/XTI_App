using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Wrapper;
using Microsoft.OData.UriParser;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_WebApp.Api;

namespace XTI_ODataQuery.Api;

public sealed class QueryToExcelApiAction<TEntity> : IAppApiAction
{
    private readonly IAppApiUser user;
    private readonly Func<QueryAction<TEntity>> createQuery;
    private readonly Func<IQueryToExcel> createQueryToExcel;

    public QueryToExcelApiAction
    (
        XtiPath path,
        ResourceAccess access,
        IAppApiUser user,
        Func<QueryAction<TEntity>> createQuery,
        Func<IQueryToExcel> createQueryToExcel,
        string friendlyName
    )
    {
        Path = path;
        Access = access;
        this.user = user;
        FriendlyName = friendlyName;
        this.createQueryToExcel = createQueryToExcel;
        this.createQuery = createQuery;
    }

    public XtiPath Path { get; }
    public string ActionName { get => Path.Action.DisplayText.Replace(" ", ""); }
    public ResourceAccess Access { get; }
    public string FriendlyName { get; }

    public async Task<WebFileResult> Execute(ODataQueryOptions options, CancellationToken stoppingToken = default)
    {
        await user.EnsureUserHasAccess(Access);
        var query = createQuery();
        var finalQuery = options.ApplyTo(query.Execute(options));
        var selectFields = options.SelectExpand.SelectExpandClause.SelectedItems
            .OfType<PathSelectItem>()
            .Select(item => item.SelectedPath)
            .OfType<ODataSelectPath>()
            .Select(item => item.FirstSegment.Identifier)
            .ToArray();
        var queryResult = new QueryResult
        (
            options.RawValues,
            selectFields,
            finalQuery
                .OfType<ISelectExpandWrapper>()
                .Select(q => q.ToDictionary())
                .ToArray()
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
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            queryToExcel.DownloadName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)
                ? queryToExcel.DownloadName
                : $"{queryToExcel.DownloadName}.xlsx"
        ); 
    }

    public AppApiActionTemplate Template()
    {
        var modelTemplate = new ValueTemplateFromType(typeof(string)).Template();
        var resultTemplate = new ValueTemplateFromType(typeof(WebFileResult)).Template();
        return new AppApiActionTemplate
        (
            Path.Action.DisplayText,
            FriendlyName,
            Access,
            modelTemplate,
            resultTemplate
        );
    }
}
