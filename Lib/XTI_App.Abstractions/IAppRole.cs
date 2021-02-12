namespace XTI_App.Abstractions
{
    public interface IAppRole
    {
        EntityID ID { get; }
        AppRoleName Name();
    }
}
