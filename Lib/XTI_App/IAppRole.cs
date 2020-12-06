namespace XTI_App
{
    public interface IAppRole
    {
        EntityID ID { get; }
        AppRoleName Name();
    }
}
