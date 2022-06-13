namespace XTI_ODataQuery.Api;

public sealed record QueryResult
(
    RawQueryValues RawQueryValues, 
    string[] SelectFields, 
    IDictionary<string, object>[] Records
);