using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XTI_App.Entities;

namespace XTI_App
{
    public sealed class ResourceGroup
    {
        private readonly AppFactory factory;
        private readonly ResourceGroupRecord record;

        internal ResourceGroup(AppFactory factory, ResourceGroupRecord record)
        {
            this.factory = factory;
            this.record = record ?? new ResourceGroupRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public ResourceGroupName Name() => new ResourceGroupName(record.Name);

        public Task<Resource> AddResource(ResourceName name) => factory.Resources().Add(this, name);

        public Task<Resource> Resource(ResourceName name) => factory.Resources().Resource(this, name);

        public Task<IEnumerable<Resource>> Resources() => factory.Resources().Resources(this);

        public override string ToString() => $"{nameof(ResourceGroup)} {ID.Value}";
    }
}
