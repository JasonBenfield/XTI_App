namespace XTI_App.Abstractions;

public interface IAppVersion
{
    EntityID ID { get; }
    AppVersionKey Key();
    Task<IResourceGroup> ResourceGroup(ResourceGroupName name);
}