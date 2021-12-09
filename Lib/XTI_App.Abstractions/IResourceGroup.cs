namespace XTI_App.Abstractions;

public interface IResourceGroup
{
    EntityID ID { get; }
    ResourceGroupName Name();
    Task<IResource> Resource(ResourceName name);
    Task<IModifierCategory> ModCategory();
}