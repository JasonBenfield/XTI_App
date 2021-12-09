using XTI_App.Abstractions;

namespace XTI_App.Api;

public interface IUserContext
{
    Task<IAppUser> User();
}