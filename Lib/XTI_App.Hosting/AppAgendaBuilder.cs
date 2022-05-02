using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Hosting;

public sealed class AppAgendaBuilder
{
    private readonly IServiceProvider sp;
    private readonly List<IAppAgendaItemBuilder> preStartBuilders = new();
    private readonly List<IAppAgendaItemBuilder> itemBuilders = new();
    private readonly List<IAppAgendaItemBuilder> postStopBuilders = new();

    public AppAgendaBuilder(IServiceProvider sp)
    {
        this.sp = sp;
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
        itemBuilders.Add(itemBuilder);
        return this;
    }

    public AppAgendaBuilder AddScheduled<TAppApi>(Action<TAppApi, ScheduledAppAgendaItemBuilder> configure)
        where TAppApi : IAppApi
    {
        var api = (TAppApi)sp.GetRequiredService<AppApiFactory>().CreateForSuperUser();
        var itemBuilder = new ScheduledAppAgendaItemBuilder();
        configure(api, itemBuilder);
        itemBuilders.Add(itemBuilder);
        return this;
    }

    public AppAgendaBuilder ApplyOptions(AppAgendaOptions options)
    {
        var immedActions = options.ImmediateItems ?? new ImmediateAppAgendaItemOptions[0];
        foreach (var immedAction in immedActions)
        {
            itemBuilders.RemoveAll(b => b.HasAction(immedAction.GroupName, immedAction.ActionName));
        }
        var schedActions = options.ScheduledItems ?? new ScheduledAppAgendaItemOptions[] { };
        foreach (var schedAction in schedActions)
        {
            itemBuilders.RemoveAll(b => b.HasAction(schedAction.GroupName, schedAction.ActionName));
        }
        var alwaysActions = options.AlwaysRunningItems ?? new AlwaysRunningAppAgendaItemOptions[] { };
        foreach (var alwaysAction in alwaysActions)
        {
            itemBuilders.RemoveAll(b => b.HasAction(alwaysAction.GroupName, alwaysAction.ActionName));
        }
        foreach (var immedAction in immedActions)
        {
            itemBuilders.Add(new ImmediateAppAgendaItemBuilder(immedAction));
        }
        foreach (var schedAction in schedActions)
        {
            itemBuilders.Add(new ScheduledAppAgendaItemBuilder(schedAction));
        }
        foreach (var alwaysAction in alwaysActions)
        {
            itemBuilders.Add(new ScheduledAppAgendaItemBuilder(alwaysAction));
        }
        return this;
    }

    public AppAgenda Build()
        => new AppAgenda
        (
            sp,
            preStartBuilders.Select(b => (ImmediateAppAgendaItem)b.Build()).ToArray(),
            itemBuilders.Select(b => b.Build()).ToArray(),
            postStopBuilders.Select(b => (ImmediateAppAgendaItem)b.Build()).ToArray()
        );
}