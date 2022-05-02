using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeAppVersion : IAppVersion
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static int NextID() => currentID.Next();

    private readonly FakeApp app;
    private readonly AppVersionKey versionKey;
    private readonly List<FakeResourceGroup> groups = new List<FakeResourceGroup>();

    public FakeAppVersion(FakeApp app, int id, AppVersionKey versionKey)
    {
        this.app = app;
        ID = id;
        this.versionKey = versionKey;
    }

    public int ID { get; }
    public AppVersionKey Key() => versionKey;

    public FakeResourceGroup AddResourceGroup(ResourceGroupName name, ModifierCategoryName categoryName)
    {
        var group = groups.FirstOrDefault(g => g.Name().Equals(name));
        if (group == null)
        {
            group = new FakeResourceGroup(app, FakeResourceGroup.NextID(), name, categoryName);
            groups.Add(group);
        }
        return group;
    }

    Task<IResourceGroup> IAppVersion.ResourceGroup(ResourceGroupName name) =>
        Task.FromResult<IResourceGroup>(ResourceGroup(name));

    public FakeResourceGroup ResourceGroup(ResourceGroupName name)
    {
        var group = groups.FirstOrDefault(g => g.Name().Equals(name));
        if (group == null)
        {
            throw new ArgumentException($"Group '{name.Value}' not found");
        }
        return group;
    }

}