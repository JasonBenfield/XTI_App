using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeAppRole : IAppRole, IEquatable<FakeAppRole>
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static int NextID() => currentID.Next();

    private readonly AppRoleName roleName;

    public FakeAppRole(int id, AppRoleName roleName)
    {
        ID = id;
        this.roleName = roleName;
    }

    public int ID { get; }

    public AppRoleName Name() => roleName;

    public override bool Equals(object? obj)
    {
        if (obj is FakeAppRole role)
        {
            return Equals(role);
        }
        return base.Equals(obj);
    }

    public override int GetHashCode() => ID.GetHashCode();

    public bool Equals(FakeAppRole? other) => ID.Equals(other?.ID);
}