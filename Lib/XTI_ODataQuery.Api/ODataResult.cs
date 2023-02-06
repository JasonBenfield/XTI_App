using System.Text.Json.Serialization;

namespace XTI_ODataQuery.Api;

internal sealed class ODataResult<TEntity>
{
    [JsonPropertyName("value")]
    public TEntity[] Records { get; set; } = new TEntity[0];
    [JsonPropertyName("@odata.count")]
    public int Count { get; set; }
}
