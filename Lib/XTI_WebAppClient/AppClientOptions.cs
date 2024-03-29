﻿using System.Text.Json;

namespace XTI_WebAppClient;

public sealed class AppClientOptions
{
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions();
    public TimeSpan Timeout { get; set; } = TimeSpan.FromHours(1);
}
