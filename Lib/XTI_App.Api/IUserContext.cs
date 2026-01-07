using XTI_App.Abstractions;

namespace XTI_App.Api;

public interface IUserContext
{
    Task<AppUserModel> User(CancellationToken ct);
    Task<AppUserModel> User(AppUserName userName, CancellationToken ct);
    Task<AppUserModel> UserOrAnon(AppUserName userName, CancellationToken ct);
    Task<AppRoleModel[]> UserRoles(AppUserModel user, ModifierModel modifier, CancellationToken ct);
}