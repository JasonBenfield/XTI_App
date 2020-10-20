using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XTI_App.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppRequest
    {
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

        private readonly AppFactory factory;
        private readonly DataRepository<AppRequestRecord> repo;
        private readonly AppRequestRecord record;

        public EntityID ID { get; }
        public XtiPath ResourceName() => XtiPath.Parse(record.Path);
        public bool HasEnded() => new Timestamp(record.TimeEnded).IsValid();

        public Task<AppVersion> Version() => factory.Versions().Version(record.VersionID);

        public Task<IEnumerable<AppEvent>> Events() => factory.Events().RetrieveByRequest(this);

        public Task<AppEvent> LogException(string eventKey, AppEventSeverity severity, DateTime timeOccurred, Exception ex, string caption)
        {
            return LogEvent
            (
                eventKey, severity, timeOccurred, caption, ex.Message, ex.StackTrace
            );
        }

        public Task<AppEvent> LogEvent(string eventKey, AppEventSeverity severity, DateTime timeOccurred, string caption, string message, string detail)
        {
            return factory.Events().LogEvent
            (
                this, eventKey, timeOccurred, severity, caption, message, detail
            );
        }

        public Task End(DateTime timeEnded)
        {
            return repo.Update(record, r =>
            {
                r.TimeEnded = timeEnded;
            });
        }

        public override string ToString() => $"{nameof(AppRequest)} {ID.Value}";

    }
}
