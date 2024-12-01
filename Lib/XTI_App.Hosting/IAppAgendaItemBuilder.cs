using XTI_App.Abstractions;

namespace XTI_App.Hosting;

public interface IAppAgendaItemBuilder
{
    internal ResourceGroupName GroupName { get; }
    internal ResourceName ActionName { get; }

    internal bool HasAction(string groupName, string actionName);

    internal AppAgendaItem Build();
}