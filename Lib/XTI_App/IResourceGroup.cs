using System.Threading.Tasks;

namespace XTI_App
{
    public interface IResourceGroup
    {
        EntityID ID { get; }
        ResourceGroupName Name();
        Task<IResource> Resource(ResourceName name);
        Task<IModifierCategory> ModCategory();
    }
}
