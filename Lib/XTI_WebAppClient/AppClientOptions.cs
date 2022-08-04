using System.Text.Json;

namespace XTI_WebAppClient;

public sealed class AppClientOptions
{
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions();
}
