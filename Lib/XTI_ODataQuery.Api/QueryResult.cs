using Microsoft.AspNetCore.OData.Query;

namespace XTI_ODataQuery.Api;

public sealed record QueryResult(ODataRawQueryOptions RawQueryValues, string[] SelectFields, IDictionary<string, object>[] Records);