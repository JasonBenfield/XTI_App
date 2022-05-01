using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeResourceGroup : IResourceGroup
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static int NextID() => currentID.Next();

    private readonly FakeApp app;
    private readonly ResourceGroupName groupName;
    private readonly ModifierCategoryName modCategoryName;
    private readonly List<FakeResource> resources = new List<FakeResource>();

    public FakeResourceGroup(FakeApp app, int id, ResourceGroupName groupName, ModifierCategoryName modCategoryName)
    {
        this.app = app;
        ID = id;
        this.groupName = groupName;
        this.modCategoryName = modCategoryName;
    }

    public int ID { get; }

    public ResourceGroupName Name() => groupName;

    Task<IModifierCategory> IResourceGroup.ModCategory() =>
        Task.FromResult<IModifierCategory>(app.ModCategory(modCategoryName));

    public FakeModifierCategory ModCategory() => app.ModCategory(modCategoryName);

    public FakeResource AddResource(ResourceName name)
    {
        var resource = resources.FirstOrDefault(r => r.Name().Equals(name));
        if (resource == null)
        {
            resource = new FakeResource(FakeResource.NextID(), name);
            resources.Add(resource);
        }
        return resource;
    }

    Task<IResource> IResourceGroup.Resource(ResourceName name) =>
        Task.FromResult<IResource>(Resource(name));

    public FakeResource Resource(ResourceName name)
    {
        var resource = resources.FirstOrDefault(r => r.Name().Equals(name));
        return resource ?? throw new ArgumentException($"Resource '{name.Value}' not found");
    }
}