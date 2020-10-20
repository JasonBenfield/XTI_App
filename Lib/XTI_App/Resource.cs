using XTI_App.Entities;

namespace XTI_App
{
    public sealed class Resource : IResource
    {
        private readonly ResourceRecord record;

        internal Resource(ResourceRecord record)
        {
            this.record = record ?? new ResourceRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public ResourceName Name() => new ResourceName(record.Name);

        public override string ToString() => $"{nameof(Resource)} {ID.Value}";
    }
}
