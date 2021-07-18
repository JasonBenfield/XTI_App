using XTI_App.Abstractions;

namespace XTI_App.Extensions
{
    internal sealed class CachedResource : IResource
    {
        private readonly ResourceName name;

        public CachedResource(IResource resource)
        {
            ID = resource.ID;
            name = resource.Name();
        }

        public EntityID ID { get; }

        public ResourceName Name() => name;
    }
}
