using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeResource : IResource
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static int NextID() => currentID.Next();

    private readonly ResourceName resourceName;

    public FakeResource(int id, ResourceName resourceName)
    {
        ID = id;
        this.resourceName = resourceName;
    }

    public int ID { get; }

    public ResourceName Name() => resourceName;
}