using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppRole : IAppRole
    {
        private readonly DataRepository<AppRoleRecord> repo;
        private readonly AppRoleRecord record;

        internal AppRole(DataRepository<AppRoleRecord> repo, AppRoleRecord record)
        {
            this.repo = repo;
            this.record = record ?? new AppRoleRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public AppRoleName Name() => new AppRoleName(record.Name);

        public bool Exists() => ID.IsValid();

        internal Task Delete() => repo.Delete(record);

        public override string ToString() => $"{nameof(AppRole)} {ID.Value}";

    }
}
