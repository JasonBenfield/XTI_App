using System.Threading.Tasks;

namespace XTI_App
{
    public interface IResourceGroup
    {
        EntityID ID { get; }
        Task<ModifierCategory> ModCategory();
    }
}
