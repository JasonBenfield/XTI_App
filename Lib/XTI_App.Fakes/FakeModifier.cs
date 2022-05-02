using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeModifier : IModifier
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static int NextID() => currentID.Next();

    private ModifierKey modKey;

    public FakeModifier(int id, ModifierKey modKey, string targetID)
    {
        ID = id;
        this.modKey = modKey;
        TargetID = targetID;
    }

    public int ID { get; internal set; }

    public string TargetID { get; }

    public ModifierKey ModKey() => modKey;

    public void SetModKey(IModifier modifier) => modKey = modifier.ModKey();
}