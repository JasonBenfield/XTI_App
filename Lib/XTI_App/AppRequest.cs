using MainDB.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XTI_App;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppRequest
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppRequestRecord> repo;
        private readonly AppRequestRecord record;

        internal AppRequest
        (
            AppFactory factory,
            DataRepository<AppRequestRecord> repo,
            AppRequestRecord record
        )
        {
            this.repo = repo;
            this.factory = factory;
            this.record = record ?? new AppRequestRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public bool HasEnded() => record.TimeEnded < DateTimeOffset.MaxValue;

        public Task<IEnumerable<AppEvent>> Events() => factory.Events().RetrieveByRequest(this);

        public Task<AppEvent> LogEvent(string eventKey, AppEventSeverity severity, DateTimeOffset timeOccurred, string caption, string message, string detail)
        {
            return factory.Events().LogEvent
            (
                this, eventKey, timeOccurred, severity, caption, message, detail
            );
        }

        public Task End(DateTimeOffset timeEnded)
        {
            return repo.Update(record, r =>
            {
                r.TimeEnded = timeEnded;
            });
        }

        public Task Edit(AppSession session, AppVersion version, Resource resource, Modifier modifier, string path, DateTimeOffset timeStarted)
            => repo.Update
            (
                record,
                r =>
                {
                    r.SessionID = session.ID.Value;
                    r.VersionID = version.ID.Value;
                    r.ResourceID = resource.ID.Value;
                    r.ModifierID = modifier.ID.Value;
                    r.Path = path ?? "";
                    r.TimeStarted = timeStarted;
                }
            );

        public override string ToString() => $"{nameof(AppRequest)} {ID.Value}";

    }
}
