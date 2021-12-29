using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeModifier : IModifier
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static EntityID NextID() => currentID.Next();

    private readonly FakeApp app;
    private readonly ModifierKey modKey;

    public FakeModifier(FakeApp app, EntityID id, ModifierKey modKey, string targetID)
    {
        this.app = app;
        ID = id;
        this.modKey = modKey;
        TargetID = targetID;
    }

    public EntityID ID { get; }

    public string TargetID { get; }

    public ModifierKey ModKey() => modKey;
}