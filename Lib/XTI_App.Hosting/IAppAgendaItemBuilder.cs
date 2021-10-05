namespace XTI_App.Hosting
{
    public interface IAppAgendaItemBuilder
    {
        bool HasAction(string groupName, string actionName);

        AppAgendaItem Build();
    }
}
