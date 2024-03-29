﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_App.Api;

namespace XTI_ConsoleApp;

public sealed class LifetimeGroup : AppApiGroupWrapper
{
    public LifetimeGroup(AppApiGroup source, IServiceProvider sp)
        : base(source)
    {
        StopApplication = source.AddAction
        (
            nameof(StopApplication), () => createStopApplication(sp)
        );
    }

    private StopApplicationAction createStopApplication(IServiceProvider sp)
    {
        var lifetime = sp.GetRequiredService<IHostApplicationLifetime>();
        return new StopApplicationAction(lifetime);
    }

    public AppApiAction<EmptyRequest, EmptyActionResult> StopApplication { get; }
}