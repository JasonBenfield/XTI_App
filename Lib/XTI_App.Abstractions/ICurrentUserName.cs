namespace XTI_App.Abstractions;

public interface ICurrentUserName
{
    Task<AppUserName> Value();
}
