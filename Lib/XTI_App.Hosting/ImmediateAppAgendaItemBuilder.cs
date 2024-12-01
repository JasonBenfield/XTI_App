using XTI_App.Abstractions;

namespace XTI_App.Hosting;

public sealed class ImmediateAppAgendaItemBuilder : IAppAgendaItemBuilder
{
    private bool isEnabled = true;

    internal ImmediateAppAgendaItemBuilder() { }

    internal ImmediateAppAgendaItemBuilder(ImmediateAppAgendaItemOptions options)
    {
        Action(new ResourceGroupName(options.GroupName), new ResourceName(options.ActionName));
        isEnabled = !options.IsDisabled;
    }

    public ResourceGroupName GroupName { get; private set; } = ResourceGroupName.Unknown;
    public ResourceName ActionName { get; private set; } = ResourceName.Unknown;

    public bool HasAction(string groupName, string actionName)
        => GroupName.Equals(groupName) && ActionName.Equals(actionName);

    public ImmediateAppAgendaItemBuilder Action(XtiPath path) => Action(path.Group, path.Action);

    public ImmediateAppAgendaItemBuilder Action(ResourceGroupName groupName, ResourceName actionName)
    {
        GroupName = groupName;
        ActionName = actionName;
        return this;
    }

    internal ImmediateAppAgendaItem Build() => new(GroupName, ActionName, isEnabled);

    AppAgendaItem IAppAgendaItemBuilder.Build() => Build();
}