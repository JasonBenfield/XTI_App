namespace XTI_App.Hosting;

public interface IAppAgendaItemBuilder
{
    internal bool HasAction(string groupName, string actionName);

    internal AppAgendaItem Build();
}