using XTI_App.Abstractions;

namespace XTI_App.Hosting;

public sealed class ImmediateAppAgendaItem : AppAgendaItem
{
    public ImmediateAppAgendaItem(ResourceGroupName groupName, ResourceName actionName, bool isDisabled)
        : base(groupName, actionName, isDisabled)
    {
    }
}