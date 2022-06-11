using ClosedXML.Excel;

namespace XTI_ODataQuery.Api;

public sealed class DefaultQueryToExcelBuilder
{
    private string downloadName = "QueryResults";
    private Action<IXLRow> formatHeaderRow = (row) => { row.Style.Font.Bold = true; };
    private Action<string, IXLColumn> formatColumn = (name, column) => { };
    private Func<string, object, object> transformData = (name, value) => value;
    private Action<string, IXLCell> formatCell = (name, cell) => { };

    public DefaultQueryToExcelBuilder SetDownloadName(string downloadName)
    {
        this.downloadName = downloadName;
        return this;
    }

    public DefaultQueryToExcelBuilder FormatHeaderRow(Action<IXLRow> formatHeaderRow)
    {
        this.formatHeaderRow = formatHeaderRow;
        return this;
    }

    public DefaultQueryToExcelBuilder FormatColumns(Action<string, IXLColumn> formatColumn)
    {
        this.formatColumn = formatColumn;
        return this;
    }

    public DefaultQueryToExcelBuilder TransformData(Func<string, object, object> transformData)
    {
        this.transformData = transformData;
        return this;
    }

    public DefaultQueryToExcelBuilder FormatCell(Action<string, IXLCell> formatCell)
    {
        this.formatCell = formatCell;
        return this;
    }

    public DefaultQueryToExcel Build() =>
        new DefaultQueryToExcel
        (
            formatHeaderRow,
            formatColumn,
            transformData,
            formatCell,
            downloadName
        );
}


public sealed class DefaultQueryToExcel : IQueryToExcel
{
    private readonly Action<IXLRow> formatHeaderRow = (row) => { row.Style.Font.Bold = true; };
    private readonly Action<string, IXLColumn> formatColumn;
    private readonly Func<string, object, object> transformData;
    private readonly Action<string, IXLCell> formatCell;

    public DefaultQueryToExcel
    (
        Action<IXLRow> formatHeaderRow,
        Action<string, IXLColumn> formatColumn,
        Func<string, object, object> transformData,
        Action<string, IXLCell> formatCell,
        string downloadName
    )
    {
        this.formatHeaderRow = formatHeaderRow;
        this.formatColumn = formatColumn;
        this.transformData = transformData;
        this.formatCell = formatCell;
        DownloadName = downloadName;
    }

    public string DownloadName { get; } = "";

    public Task<Stream> ToExcel(QueryResult queryResult, CancellationToken ct)
    {
        using var workbook = new XLWorkbook(XLEventTracking.Disabled);
        var dataWS = workbook.AddWorksheet("Data");
        var rowIndex = 1;
        addRow
        (
            dataWS,
            rowIndex,
            queryResult.SelectFields.ToArray()
        );
        formatHeaderRow(dataWS.Row(rowIndex));
        var columnIndex = 1;
        foreach (var field in queryResult.SelectFields)
        {
            formatColumn(field, dataWS.Column(columnIndex));
            columnIndex++;
        }
        rowIndex++;
        foreach (var record in queryResult.Records)
        {
            columnIndex = 1;
            foreach (var field in queryResult.SelectFields)
            {
                var value = transformData(field, record[field]);
                var cell = dataWS.Cell(rowIndex, columnIndex);
                if (value is string str)
                {
                    cell.SetValue(str);
                }
                else
                {
                    cell.Value = value;
                }
                formatCell(field, cell);
                columnIndex++;
            }
            rowIndex++;
        }
        var table = dataWS.Range
        (
            dataWS.Cell(1, 1),
            dataWS.Cell(rowIndex - 1, queryResult.SelectFields.Length)
        )
        .CreateTable();
        dataWS.Columns().AdjustToContents();

        var queryWS = workbook.AddWorksheet("Parameters");
        rowIndex = 1;
        if (!string.IsNullOrWhiteSpace(queryResult.RawQueryValues.Select))
        {
            addRow
            (
                queryWS,
                rowIndex,
                "Select",
                queryResult.RawQueryValues.Select
            );
        }
        if (!string.IsNullOrWhiteSpace(queryResult.RawQueryValues.Filter))
        {
            addRow
            (
                queryWS,
                rowIndex,
                "Filter",
                queryResult.RawQueryValues.Filter
            );
        }
        if (!string.IsNullOrWhiteSpace(queryResult.RawQueryValues.OrderBy))
        {
            addRow
            (
                queryWS,
                rowIndex,
                "Order By",
                queryResult.RawQueryValues.OrderBy
            );
        }

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult<Stream>(stream);
    }

    private void addRow(IXLWorksheet worksheet, int rowIndex, params object[] values)
    {
        var columnIndex = 1;
        foreach (var value in values)
        {
            if (value is string str)
            {
                worksheet.Cell(rowIndex, columnIndex).SetValue(str);
            }
            else
            {
                worksheet.Cell(rowIndex, columnIndex).Value = value;
            }
            columnIndex++;
        }
    }

}
