using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeResourceGroup : IResourceGroup
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static EntityID NextID() => currentID.Next();

    private readonly FakeApp app;
    private readonly ResourceGroupName groupName;
    private readonly ModifierCategoryName modCategoryName;
    private readonly List<FakeResource> resources = new List<FakeResource>();

    public FakeResourceGroup(FakeApp app, EntityID id, ResourceGroupName groupName, ModifierCategoryName modCategoryName)
    {
        this.app = app;
        ID = id;
        this.groupName = groupName;
        this.modCategoryName = modCategoryName;
    }

    public EntityID ID { get; }

    public ResourceGroupName Name() => groupName;

    public async Task<IModifierCategory> ModCategory() => await app.ModCategory(modCategoryName);

    public FakeResource AddResource(ResourceName name)
    {
        var resource = new FakeResource(FakeResource.NextID(), name);
        resources.Add(resource);
        return resource;
    }

    public Task<IResource> Resource(ResourceName name)
    {
        var resource = resources.FirstOrDefault(r => r.Name().Equals(name));
        if(resource == null) { throw new ArgumentException($"Resource '{name.Value}' not found"); }
        return Task.FromResult<IResource>(resource);
    }
}