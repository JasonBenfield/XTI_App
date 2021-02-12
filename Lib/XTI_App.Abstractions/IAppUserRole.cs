namespace XTI_App.Abstractions
{
    public interface IAppUserRole
    {
        int RoleID { get; }
        bool IsRole(IAppRole appRole);
    }
}
