using XTI_App.Abstractions;

namespace XTI_App.Api;

public interface ISourceUserContext : IUserContext
{
    Task<AppUserName> CurrentUserName();
}