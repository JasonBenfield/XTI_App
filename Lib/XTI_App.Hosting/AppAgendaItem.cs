using XTI_App.Abstractions;

namespace XTI_App.Hosting;

public class AppAgendaItem
{
    protected AppAgendaItem(ResourceGroupName groupName, ResourceName actionName, bool isEnsabled)
    {
        GroupName = groupName;
        ActionName = actionName;
        IsEnabled = isEnsabled;
    }

    public ResourceGroupName GroupName { get; }
    public ResourceName ActionName { get; }
    public bool IsEnabled { get; }
}