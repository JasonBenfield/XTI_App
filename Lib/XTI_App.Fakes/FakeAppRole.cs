using XTI_App.Abstractions;

namespace XTI_App.Fakes
{
    public sealed class FakeAppRole : IAppRole
    {
        private static FakeEntityID currentID = new FakeEntityID();
        public static EntityID NextID() => currentID.Next();

        private readonly AppRoleName roleName;

        public FakeAppRole(EntityID id, AppRoleName roleName)
        {
            ID = id;
            this.roleName = roleName;
        }

        public EntityID ID { get; }

        public AppRoleName Name() => roleName;
    }
}
