using XTI_App.Abstractions;

namespace XTI_App.Hosting;

public sealed class ImmediateAppAgendaItemBuilder : IAppAgendaItemBuilder
{
    private ResourceGroupName groupName = ResourceGroupName.Unknown;
    private ResourceName actionName = ResourceName.Unknown;
    private bool isEnabled = true;

    internal ImmediateAppAgendaItemBuilder() { }

    internal ImmediateAppAgendaItemBuilder(ImmediateAppAgendaItemOptions options)
    {
        Action(new ResourceGroupName(options.GroupName), new ResourceName(options.ActionName));
        if (options.IsDisabled)
        {
            Disable();
        }
    }

    public bool HasAction(string groupName, string actionName)
        => this.groupName.Equals(groupName) && this.actionName.Equals(actionName);

    public ImmediateAppAgendaItemBuilder Action(XtiPath path) => Action(path.Group, path.Action);

    public ImmediateAppAgendaItemBuilder Action(ResourceGroupName groupName, ResourceName actionName)
    {
        this.groupName = groupName;
        this.actionName = actionName;
        return this;
    }

    public bool Disable() => isEnabled = false;

    public ImmediateAppAgendaItem Build()
        => new ImmediateAppAgendaItem(groupName, actionName, isEnabled);

    AppAgendaItem IAppAgendaItemBuilder.Build() => Build();
}