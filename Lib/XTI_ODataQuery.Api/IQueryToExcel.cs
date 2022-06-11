namespace XTI_ODataQuery.Api;

public interface IQueryToExcel
{
    string DownloadName { get; }

    Task<Stream> ToExcel(QueryResult queryResult, CancellationToken ct);
}
