using ClosedXML.Excel;
using NUnit.Framework;
using XTI_ODataQuery.Api;

namespace XTI_ODataQueryTests;

public sealed class DefaultQueryToExcelTest
{
    [Test]
    public async Task ShouldOutputToExcel()
    {
        var queryToExcel = new DefaultQueryToExcelBuilder()
            .FormatColumns
            (
                (string field, IXLColumn column) =>
                {
                    if(field == "TimeAdded")
                    {
                        column.Style.DateFormat.Format = "M/dd/yy";
                    }
                }
            )
            .Build();
        var selectFields = new[] { "ID", "DisplayText", "TimeAdded" };
        var queryResult = new QueryResult
        (
            new RawQueryValues
            (
                string.Join(",", selectFields),
                "ID le 20",
                "TimeAdded asc",
                "",
                ""
            ),
            selectFields,
            Enumerable.Range(1, 20)
                .Select
                (
                    i => new Dictionary<string, object>
                    {
                        { "ID", i },
                        { "DisplayText", $"Whatever {i}" },
                        { "TimeAdded", DateTimeOffset.Now }
                    }
                )
                .ToArray()
        );
        using var excelStream = await queryToExcel.ToExcel(queryResult, CancellationToken.None);
        var dir = Path.Combine(Path.GetTempPath(), "xti");
        if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
        var path = Path.Combine(dir, "QueryResults.xlsx");
        using var streamWriter = new StreamWriter(path, false);
        await excelStream.CopyToAsync(streamWriter.BaseStream);
        streamWriter.Close();
        Console.WriteLine($"Excel file: {path}");
    }
}