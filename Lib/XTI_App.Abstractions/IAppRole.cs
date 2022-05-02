namespace XTI_App.Abstractions;

public interface IAppRole
{
    int ID { get; }
    AppRoleName Name();
}