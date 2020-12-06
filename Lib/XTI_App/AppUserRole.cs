using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppUserRole : IAppUserRole
    {
        private readonly DataRepository<AppUserRoleRecord> repo;
        private readonly AppUserRoleRecord record;

        internal AppUserRole(DataRepository<AppUserRoleRecord> repo, AppUserRoleRecord record)
        {
            this.repo = repo;
            this.record = record ?? new AppUserRoleRecord();
        }

        public int RoleID { get => record.RoleID; }
        public bool IsRole(IAppRole appRole) => appRole.ID.Value == RoleID;

        internal Task Delete() => repo.Delete(record);

        public override string ToString() => $"{nameof(AppUserRole)} {record.ID}";

    }
}
