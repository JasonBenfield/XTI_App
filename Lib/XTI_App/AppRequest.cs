using MainDB.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppRequest
    {
        private readonly AppFactory factory;
        private readonly AppRequestRecord record;

        internal AppRequest
        (
            AppFactory factory,
            AppRequestRecord record
        )
        {
            this.factory = factory;
            this.record = record ?? new AppRequestRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public bool HasEnded() => record.TimeEnded < DateTimeOffset.MaxValue;

        public bool HappendOnOrBefore(DateTimeOffset before)
        {
            DateTimeOffset date;
            if (HasEnded())
            {
                date = record.TimeEnded;
            }
            else
            {
                date = record.TimeStarted;
            }
            return date <= before;
        }

        public Task<AppEvent[]> Events() => factory.Events().RetrieveByRequest(this);

        public Task<AppEvent> LogEvent(string eventKey, AppEventSeverity severity, DateTimeOffset timeOccurred, string caption, string message, string detail)
        {
            return factory.Events().LogEvent
            (
                this, eventKey, timeOccurred, severity, caption, message, detail
            );
        }

        public Task End(DateTimeOffset timeEnded)
            => factory.DB
                .Requests
                .Update(record, r =>
                {
                    r.TimeEnded = timeEnded;
                });

        public Task Edit(AppSession session, Resource resource, Modifier modifier, string path, DateTimeOffset timeStarted)
            => factory.DB
                .Requests
                .Update
                (
                    record,
                    r =>
                    {
                        r.SessionID = session.ID.Value;
                        r.ResourceID = resource.ID.Value;
                        r.ModifierID = modifier.ID.Value;
                        r.Path = path ?? "";
                        r.TimeStarted = timeStarted;
                    }
                );

        public AppRequestModel ToModel() => new AppRequestModel
        {
            ID = ID.Value,
            SessionID = record.SessionID,
            Path = record.Path,
            ResourceID = record.ResourceID,
            ModifierID = record.ModifierID,
            TimeStarted = record.TimeStarted,
            TimeEnded = record.TimeEnded
        };

        public override string ToString() => $"{nameof(AppRequest)} {ID.Value}";

    }
}
