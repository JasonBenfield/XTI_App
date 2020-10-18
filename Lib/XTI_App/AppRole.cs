using System.Threading.Tasks;
using XTI_App.Entities;
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
        }

        public int ID { get => record.ID; }
        public AppRoleName Name() => new AppRoleName(record.Name);

        public bool Exists() => ID > 0;

        internal Task Delete() => repo.Delete(record);

        public override string ToString() => $"{nameof(AppRole)} {ID}";

    }
}
