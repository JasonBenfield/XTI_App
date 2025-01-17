﻿using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;
using XTI_TempLog;

namespace XTI_App.Hosting;

public sealed class AppAgendaBuilder
{
    private readonly IServiceProvider sp;
    private readonly IClock clock;
    private readonly XtiEnvironment xtiEnv;
    private readonly XtiBasePath xtiBasePath;
    private readonly TempLogRepository tempLogRepo;
    private readonly List<IAppAgendaItemBuilder> preStartBuilders = new();
    private readonly List<IAppAgendaItemBuilder> itemBuilders = new();
    private readonly List<IAppAgendaItemBuilder> postStopBuilders = new();

    public AppAgendaBuilder
    (
        IServiceProvider sp,
        IClock clock,
        XtiEnvironment xtiEnv,
        XtiBasePath xtiBasePath,
        TempLogRepository tempLogRepo
    )
    {
        this.sp = sp;
        this.clock = clock;
        this.xtiEnv = xtiEnv;
        this.xtiBasePath = xtiBasePath;
        this.tempLogRepo = tempLogRepo;
    }

    public AppAgendaBuilder AddPreStart<TAppApi>(Func<TAppApi, AppApiAction<EmptyRequest, EmptyActionResult>> createAction)
        where TAppApi : IAppApi
    {
        var api = (TAppApi)sp.GetRequiredService<AppApiFactory>().CreateForSuperUser();
        var preStartBuilder = new ImmediateAppAgendaItemBuilder();
        preStartBuilder.Action(createAction(api).Path);
        preStartBuilders.Add(preStartBuilder);
        return this;
    }

    public AppAgendaBuilder AddPostStop<TAppApi>(Func<TAppApi, AppApiAction<EmptyRequest, EmptyActionResult>> createAction)
        where TAppApi : IAppApi
    {
        var api = (TAppApi)sp.GetRequiredService<AppApiFactory>().CreateForSuperUser();
        var postStopBuilder = new ImmediateAppAgendaItemBuilder();
        postStopBuilder.Action(createAction(api).Path);
        postStopBuilders.Add(postStopBuilder);
        return this;
    }

    public AppAgendaBuilder AddImmediate<TAppApi>(Func<TAppApi, AppApiAction<EmptyRequest, EmptyActionResult>> createAction)
        where TAppApi : IAppApi
    {
        var api = (TAppApi)sp.GetRequiredService<AppApiFactory>().CreateForSuperUser();
        var itemBuilder = new ImmediateAppAgendaItemBuilder();
        itemBuilder.Action(createAction(api).Path);
        itemBuilders.RemoveAll(b => b is ImmediateAppAgendaItemBuilder && b.HasAction(itemBuilder.GroupName.DisplayText, itemBuilder.ActionName.DisplayText));
        itemBuilders.Add(itemBuilder);
        return this;
    }

    public AppAgendaBuilder AddScheduled<TAppApi>(Action<TAppApi, ScheduledAppAgendaItemBuilder> configure)
        where TAppApi : IAppApi
    {
        var api = (TAppApi)sp.GetRequiredService<AppApiFactory>().CreateForSuperUser();
        var itemBuilder = new ScheduledAppAgendaItemBuilder();
        configure(api, itemBuilder);
        itemBuilders.RemoveAll(b => b is ScheduledAppAgendaItemBuilder && b.HasAction(itemBuilder.GroupName.DisplayText, itemBuilder.ActionName.DisplayText));
        itemBuilders.Add(itemBuilder);
        return this;
    }

    public AppAgendaBuilder ApplyOptions(AppAgendaOptions options)
    {
        var immedActions = options.ImmediateItems ?? [];
        foreach (var immedAction in immedActions)
        {
            itemBuilders.RemoveAll(b => b.HasAction(immedAction.GroupName, immedAction.ActionName));
        }
        foreach (var immedAction in immedActions)
        {
            itemBuilders.Add(new ImmediateAppAgendaItemBuilder(immedAction));
        }
        var schedActions = options.ScheduledItems ?? [];
        ApplyOptions(schedActions);
        var alwaysActions = options.AlwaysRunningItems ?? [];
        foreach (var alwaysAction in alwaysActions)
        {
            itemBuilders.RemoveAll(b => b.HasAction(alwaysAction.GroupName, alwaysAction.ActionName));
        }
        foreach (var alwaysAction in alwaysActions)
        {
            itemBuilders.Add(new ScheduledAppAgendaItemBuilder(alwaysAction));
        }
        return this;
    }

    public void ApplyOptions(ScheduledAppAgendaItemOptions[] schedActions)
    {
        foreach (var schedAction in schedActions)
        {
            itemBuilders.RemoveAll(b => b.HasAction(schedAction.GroupName, schedAction.ActionName));
        }
        foreach (var schedAction in schedActions)
        {
            itemBuilders.Add(new ScheduledAppAgendaItemBuilder(schedAction));
        }
    }

    internal AppAgenda Build() =>
        new
        (
            sp,
            clock,
            xtiEnv,
            xtiBasePath,
            tempLogRepo,
            preStartBuilders.Select(b => (ImmediateAppAgendaItem)b.Build()).ToArray(),
            itemBuilders.Select(b => b.Build()).ToArray(),
            postStopBuilders.Select(b => (ImmediateAppAgendaItem)b.Build()).ToArray()
        );
}