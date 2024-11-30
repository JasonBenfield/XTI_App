using System.Text.Json;
using System.Text.Json.Serialization;
using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class AppClientOptions
{
    public AppClientOptions()
        : this(new EmptyAppClientSessionKey(), new EmptyAppClientRequestKey())
    {
    }

    public AppClientOptions(IAppClientSessionKey sessionKey, IAppClientRequestKey requestKey)
    {
        SessionKey = sessionKey;
        RequestKey = requestKey;
        JsonSerializerOptions = new JsonSerializerOptions();
        JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        JsonSerializerOptions.PropertyNamingPolicy = null;
        JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        JsonSerializerOptions.AddCoreConverters();
    }

    public JsonSerializerOptions JsonSerializerOptions { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromHours(1);
    public IAppClientSessionKey SessionKey { get; set; }
    public IAppClientRequestKey RequestKey { get; set; }
}
