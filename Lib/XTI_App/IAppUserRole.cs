namespace XTI_App
{
    public interface IAppUserRole
    {
        int RoleID { get; }
        bool IsRole(IAppRole appRole);
        AccessModifier Modifier();
    }
}
