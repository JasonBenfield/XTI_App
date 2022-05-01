namespace XTI_App.Abstractions;

public interface IAppUser
{
    int ID { get; }
    AppUserName UserName();
    Task<IAppRole[]> Roles(IModifier modifier);
}