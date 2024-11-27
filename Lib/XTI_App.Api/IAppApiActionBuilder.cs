namespace XTI_App.Api;

public interface IAppApiActionBuilder
{
    public string ActionName { get; }

    IAppApiAction Build();
}
