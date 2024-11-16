using XTI_App.Abstractions;

namespace XTI_App.Api;

public interface IUserContext
{
    Task<AppUserModel> User();
    Task<AppUserModel> User(AppUserName userName);
    Task<AppUserModel> UserOrAnon(AppUserName userName);
    Task<AppRoleModel[]> UserRoles(AppUserModel user, ModifierModel modifier);
}