using ClosedXML.Excel;

namespace XTI_ODataQuery.Api;

public sealed class DefaultQueryToExcelBuilder
{
    private string downloadName = "QueryResults";
    private Action<IXLRow> formatHeaderRow = (row) => { row.Style.Font.Bold = true; };
    private Action<string, IXLCell> formatHeaderCell = (name, cell) => { };
    private Action<string, IXLColumn> formatColumn = (name, column) => { };
    private Func<string, object, object> transformData = (name, value) => value;
    private Action<string, IXLCell> formatDataCell = (name, cell) => { };
    private Action<IDictionary<string, object>, IXLRow> formatRecordRow = (dict, row) => { };
    private Action<IXLTable> formatTable = (_) => { };
    private Action<QueryResult, IXLWorkbook> formatWorkbook = (result, wkbk) => { };
    private bool includeParameters = true;

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

    public DefaultQueryToExcelBuilder FormatHeaderCell(Action<string, IXLCell> formatHeaderCell)
    {
        this.formatHeaderCell = formatHeaderCell;
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

    public DefaultQueryToExcelBuilder FormatCell(Action<string, IXLCell> formatDataCell)
    {
        this.formatDataCell = formatDataCell;
        return this;
    }

    public DefaultQueryToExcelBuilder FormatRecordRow(Action<IDictionary<string, object>, IXLRow> formatRecordRow)
    {
        this.formatRecordRow = formatRecordRow;
        return this;
    }

    public DefaultQueryToExcelBuilder FormatTable(Action<IXLTable> formatTable)
    {
        this.formatTable = formatTable;
        return this;
    }

    public DefaultQueryToExcelBuilder FormatWorkbook(Action<QueryResult, IXLWorkbook> formatWorkbook)
    {
        this.formatWorkbook = formatWorkbook;
        return this;
    }

    public DefaultQueryToExcelBuilder HideParameters()
    {
        includeParameters = false;
        return this;
    }

    public DefaultQueryToExcel Build() =>
        new DefaultQueryToExcel
        (
            formatHeaderRow,
            formatHeaderCell,
            formatColumn,
            transformData,
            formatDataCell,
            formatRecordRow,
            formatTable,
            formatWorkbook,
            includeParameters,
            downloadName
        );
}
