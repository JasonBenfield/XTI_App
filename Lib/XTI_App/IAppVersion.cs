using System.Threading.Tasks;

namespace XTI_App
{
    public interface IAppVersion
    {
        EntityID ID { get; }
        AppVersionKey Key();
        Task<IResourceGroup> ResourceGroup(ResourceGroupName name);
    }
}
