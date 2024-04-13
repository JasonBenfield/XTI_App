﻿using XTI_Core;

namespace XTI_App.Api;

public sealed class DefaultServiceAppOptions
{
    public XtiTokenOptions XtiToken { get; set; } = new();
    public HubClientOptions HubClient { get; set; } = new();
    public DbOptions DB { get; set; } = new();
}
