using MainDB.Entities;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class AppRole : IAppRole
    {
        private readonly AppFactory factory;
        private readonly AppRoleRecord record;

        internal AppRole(AppFactory factory, AppRoleRecord record)
        {
            this.factory = factory;
            this.record = record ?? new AppRoleRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public AppRoleName Name() => new AppRoleName(record.Name);

        public bool Exists() => ID.IsValid();

        public bool IsDeactivated() => record.TimeDeactivated < DateTimeOffset.MaxValue;

        internal Task Deactivate(DateTimeOffset timeDeactivated)
            => updateTimeDeactivated(timeDeactivated);

        internal Task Activate() => updateTimeDeactivated(DateTimeOffset.MaxValue);

        private Task updateTimeDeactivated(DateTimeOffset timeDeactivated)
            => factory.DB.Roles.Update(record, r => r.TimeDeactivated = timeDeactivated);

        internal Task<App> App() => factory.Apps().App(record.AppID);

        public AppRoleModel ToModel() => new AppRoleModel
        {
            ID = ID.Value,
            Name = Name().DisplayText
        };

        public override string ToString() => $"{nameof(AppRole)} {ID.Value}";

    }
}
