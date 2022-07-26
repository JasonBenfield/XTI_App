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
        isEnabled = !options.IsDisabled;
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

    internal ImmediateAppAgendaItem Build()
        => new ImmediateAppAgendaItem(groupName, actionName, isEnabled);

    AppAgendaItem IAppAgendaItemBuilder.Build() => Build();
}