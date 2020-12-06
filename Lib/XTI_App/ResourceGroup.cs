using System.Collections.Generic;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class ResourceGroup : IResourceGroup
    {
        private readonly DataRepository<ResourceGroupRecord> repo;
        private readonly AppFactory factory;
        private readonly ResourceGroupRecord record;

        internal ResourceGroup(DataRepository<ResourceGroupRecord> repo, AppFactory factory, ResourceGroupRecord record)
        {
            this.repo = repo;
            this.factory = factory;
            this.record = record ?? new ResourceGroupRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public ResourceGroupName Name() => new ResourceGroupName(record.Name);

        public async Task<Resource> TryAddResource(ResourceName name)
        {
            var resource = await Resource(name);
            if (!resource.Name().Equals(name))
            {
                resource = await AddResource(name);
            }
            return resource;
        }

        private Task<Resource> AddResource(ResourceName name) => factory.Resources().Add(this, name);

        async Task<IResource> IResourceGroup.Resource(ResourceName name) => await Resource(name);

        public Task<Resource> Resource(ResourceName name) => factory.Resources().Resource(this, name);

        public Task<IEnumerable<Resource>> Resources() => factory.Resources().Resources(this);

        public async Task<IEnumerable<Modifier>> Modifiers()
        {
            var modCategory = await factory.ModCategories().Category(record.ModCategoryID);
            var modifiers = await modCategory.Modifiers();
            return modifiers;
        }

        public async Task<Modifier> Modifier(ModifierKey modKey)
        {
            var modCategory = await factory.ModCategories().Category(record.ModCategoryID);
            var modifier = await modCategory.Modifier(modKey);
            return modifier;
        }

        public Task SetModCategory(ModifierCategory category)
            => repo.Update(record, r =>
            {
                r.ModCategoryID = category.ID.Value;
            });

        async Task<IModifierCategory> IResourceGroup.ModCategory() => await ModCategory();

        public Task<ModifierCategory> ModCategory()
            => factory.ModCategories().Category(record.ModCategoryID);

        public override string ToString() => $"{nameof(ResourceGroup)} {ID.Value}";
    }
}
