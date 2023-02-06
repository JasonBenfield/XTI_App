using ClosedXML.Excel;

namespace XTI_ODataQuery.Api;

public sealed class DefaultQueryToExcel : IQueryToExcel
{
    private readonly Action<IXLRow> formatHeaderRow;
    private readonly Action<string, IXLColumn> formatColumn;
    private readonly Action<string, IXLCell> formatHeaderCell;
    private readonly Func<string, object, object> transformData;
    private readonly Action<string, IXLCell> formatDataCell;
    private readonly Action<IDictionary<string, object>, IXLRow> formatRecordRow;
    private readonly Action<IXLTable> formatTable;
    private readonly Action<QueryResult, IXLWorkbook> formatWorkbook;
    private readonly bool includeParameters;

    public DefaultQueryToExcel
    (
        Action<IXLRow> formatHeaderRow,
        Action<string, IXLCell> formatHeaderCell,
        Action<string, IXLColumn> formatColumn,
        Func<string, object, object> transformData,
        Action<string, IXLCell> formatDataCell,
        Action<IDictionary<string, object>, IXLRow> formatRecordRow,
        Action<IXLTable> formatTable,
        Action<QueryResult, IXLWorkbook> formatWorkbook,
        bool includeParameters,
        string downloadName
    )
    {
        this.formatHeaderRow = formatHeaderRow;
        this.formatHeaderCell = formatHeaderCell;
        this.formatColumn = formatColumn;
        this.transformData = transformData;
        this.formatDataCell = formatDataCell;
        this.formatRecordRow = formatRecordRow;
        this.formatTable = formatTable;
        this.formatWorkbook = formatWorkbook;
        this.includeParameters = includeParameters;
        DownloadName = downloadName;
    }

    public string DownloadName { get; }

    public Task<Stream> ToExcel(QueryResult queryResult, CancellationToken ct)
    {
        using var workbook = new XLWorkbook();
        var dataWS = workbook.AddWorksheet("Data");
        var rowIndex = 1;
        var columnIndex = 1;
        if (queryResult.Records.Any())
        {
            foreach (var field in queryResult.SelectFields)
            {
                var cell = dataWS.Cell(rowIndex, columnIndex);
                cell.SetValue(field);
                formatHeaderCell(field, cell);
                formatColumn(field, dataWS.Column(columnIndex));
                columnIndex++;
            }
            formatHeaderRow(dataWS.Row(rowIndex));
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
                    else if (value is bool boolVal)
                    {
                        cell.SetValue(boolVal);
                    }
                    else if (value is short shortVal)
                    {
                        cell.SetValue(shortVal);
                    }
                    else if (value is int intVal)
                    {
                        cell.SetValue(intVal);
                    }
                    else if (value is long longVal)
                    {
                        cell.SetValue(longVal);
                    }
                    else if (value is decimal decVal)
                    {
                        cell.SetValue(decVal);
                    }
                    else if (value is double dblVal)
                    {
                        cell.SetValue(dblVal);
                    }
                    else if (value is float fltVal)
                    {
                        cell.SetValue(fltVal);
                    }
                    else if (value is DateOnly dVal)
                    {
                        cell.SetValue(dVal.ToDateTime(new TimeOnly()));
                    }
                    else if (value is DateTime dtVal)
                    {
                        cell.SetValue(dtVal);
                    }
                    else if (value is DateTimeOffset dtoVal)
                    {
                        cell.SetValue(dtoVal.LocalDateTime);
                    }
                    else if (value != null)
                    {
                        cell.SetValue(value.ToString());
                    }
                    formatDataCell(field, cell);
                    columnIndex++;
                }
                formatRecordRow(record, dataWS.Row(rowIndex));
                rowIndex++;
            }
        }
        else
        {
            dataWS.Row(rowIndex).Cell(1).SetValue("No records were found.");
        }
        var table = dataWS.Range
        (
            dataWS.Cell(1, 1),
            dataWS.Cell(rowIndex - 1, queryResult.SelectFields.Length)
        )
        .CreateTable();
        table.Theme = XLTableTheme.TableStyleLight9;
        formatTable(table);
        dataWS.Columns().AdjustToContents();
        if (includeParameters)
        {
            AddParamtersWS(queryResult, workbook);
        }
        formatWorkbook(queryResult, workbook);
        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return Task.FromResult<Stream>(stream);
    }

    private void AddParamtersWS(QueryResult queryResult, XLWorkbook workbook)
    {
        int rowIndex;
        var queryWS = workbook.AddWorksheet("Parameters");
        rowIndex = 1;
        var queryHeader = queryWS.Cell(1, 1);
        queryHeader.Style.Font.Bold = true;
        queryHeader.SetValue("Query Options");
        queryWS.Range(queryHeader, queryWS.Cell(1, 2)).Merge();
        rowIndex++;
        if (!string.IsNullOrWhiteSpace(queryResult.RawQueryValues.Select))
        {
            AddQueryValue
            (
                queryWS,
                rowIndex,
                "Select",
                queryResult.RawQueryValues.Select
            );
            rowIndex++;
        }
        if (!string.IsNullOrWhiteSpace(queryResult.RawQueryValues.Filter))
        {
            AddQueryValue
            (
                queryWS,
                rowIndex,
                "Filter",
                queryResult.RawQueryValues.Filter
            );
            rowIndex++;
        }
        if (!string.IsNullOrWhiteSpace(queryResult.RawQueryValues.OrderBy))
        {
            AddQueryValue
            (
                queryWS,
                rowIndex,
                "Order By",
                queryResult.RawQueryValues.OrderBy
            );
            rowIndex++;
        }
        if (!string.IsNullOrWhiteSpace(queryResult.RawQueryValues.Skip))
        {
            AddQueryValue
            (
                queryWS,
                rowIndex,
                "Skip",
                queryResult.RawQueryValues.Skip
            );
            rowIndex++;
        }
        if (!string.IsNullOrWhiteSpace(queryResult.RawQueryValues.Top))
        {
            AddQueryValue
            (
                queryWS,
                rowIndex,
                "Top",
                queryResult.RawQueryValues.Top
            );
        }
        queryWS.Column(1).AdjustToContents();
        queryWS.Column(1).Style.Font.Bold = true;
        queryWS.Column(2).AdjustToContents();
    }

    private void AddQueryValue(IXLWorksheet worksheet, int rowIndex, string caption, string value)
    {
        var captionCell = worksheet.Cell(rowIndex, 1);
        captionCell.SetValue(caption);
        var valueCell = worksheet.Cell(rowIndex, 2);
        valueCell.SetValue(value);
    }

}
