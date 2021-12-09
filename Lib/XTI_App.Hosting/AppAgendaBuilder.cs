using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Hosting;

public sealed class AppAgendaBuilder
{
    private readonly IServiceProvider sp;
    private readonly List<IAppAgendaItemBuilder> itemBuilders = new List<IAppAgendaItemBuilder>();

    public AppAgendaBuilder(IServiceProvider sp)
    {
        this.sp = sp;
    }

    public AppAgendaBuilder AddImmediate(ResourceGroupName groupName, ResourceName actionName)
        => AddImmediate(b => b.Action(groupName, actionName));

    public AppAgendaBuilder AddImmediate(XtiPath path)
        => AddImmediate(b => b.Action(path));

    public AppAgendaBuilder AddImmediate(Action<ImmediateAppAgendaItemBuilder> configure)
    {
        var itemBuilder = new ImmediateAppAgendaItemBuilder();
        configure(itemBuilder);
        itemBuilders.Add(itemBuilder);
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

    public AppAgendaBuilder AddScheduled(Action<ScheduledAppAgendaItemBuilder> configure)
    {
        var itemBuilder = new ScheduledAppAgendaItemBuilder();
        configure(itemBuilder);
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
        var immedActions = options.ImmediateItems ?? new ImmediateAppAgendaItemOptions[] { };
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
            itemBuilders.Select(b => b.Build()).ToArray()
        );
}