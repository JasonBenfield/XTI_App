namespace XTI_App.Abstractions;

public interface IAppVersion
{
    int ID { get; }
    AppVersionKey Key();
    Task<IResourceGroup> ResourceGroup(ResourceGroupName name);
}