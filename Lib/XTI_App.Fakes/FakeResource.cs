using System;
using XTI_App.Abstractions;

namespace XTI_App.Fakes
{
    public sealed class FakeResource : IResource
    {
        private static FakeEntityID currentID = new FakeEntityID();
        public static EntityID NextID() => currentID.Next();

        private readonly ResourceName resourceName;

        public FakeResource(EntityID id, ResourceName resourceName)
        {
            ID = id;
            this.resourceName = resourceName;
        }

        public EntityID ID { get; }

        public ResourceName Name() => resourceName;
    }
}
