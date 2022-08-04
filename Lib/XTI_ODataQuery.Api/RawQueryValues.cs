using Microsoft.AspNetCore.OData.Query;

namespace XTI_ODataQuery.Api;

public sealed record RawQueryValues(string Select, string Filter, string OrderBy, string Skip, string Top)
{
    public RawQueryValues(ODataQueryOptions query)
        : this(query.RawValues)
    {
    }

    public RawQueryValues(ODataRawQueryOptions rawQuery)
        : this(rawQuery.Select, rawQuery.Filter, rawQuery.OrderBy, rawQuery.Skip, rawQuery.Top)
    {
    }

    public string ToQuery() =>
        string.Join
        (
            "&",
            new[]
            {
                string.IsNullOrWhiteSpace(Select) ? null : $"$select={Select}",
                string.IsNullOrWhiteSpace(Filter) ? null : $"$filter={Filter}",
                string.IsNullOrWhiteSpace(OrderBy) ? null : $"$orderby={OrderBy}",
                string.IsNullOrWhiteSpace(Skip) ? null : $"$skip={Skip}",
                string.IsNullOrWhiteSpace(Top) ? null : $"$top={Top}"
            }
            .Where(str => str != null)
        );
}