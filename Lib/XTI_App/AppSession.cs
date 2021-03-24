using MainDB.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class AppSession
    {
        private readonly AppFactory factory;
        private readonly AppSessionRecord record;

        internal AppSession(AppFactory factory, AppSessionRecord record)
        {
            this.factory = factory;
            this.record = record ?? new AppSessionRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public int UserID { get => record.UserID; }

        public bool HasStarted() => record.TimeStarted > DateTimeOffset.MinValue;
        public bool HasEnded() => record.TimeEnded < DateTimeOffset.MaxValue;

        public Task<AppRequest> LogRequest
        (
            string requestKey,
            Resource resource,
            Modifier modifier,
            string path,
            DateTimeOffset timeRequested
        )
        {
            var requestRepo = factory.Requests();
            return requestRepo.Add(this, requestKey, resource, modifier, path, timeRequested);
        }

        public Task Edit(AppUser user, DateTimeOffset timeStarted, string requesterKey, string userAgent, string remoteAddress)
            => factory.DB
                .Sessions
                .Update
                (
                    record,
                    r =>
                    {
                        r.UserID = user.ID.Value;
                        r.TimeStarted = timeStarted;
                        r.RequesterKey = requesterKey;
                        r.UserAgent = userAgent ?? "";
                        r.RemoteAddress = remoteAddress ?? "";
                    }
                );

        public Task Authenticate(IAppUser user)
            => factory.DB
                .Sessions
                .Update(record, r =>
                {
                    r.UserID = user.ID.Value;
                });

        public Task End(DateTimeOffset timeEnded)
            => factory.DB
                .Sessions
                .Update(record, r =>
                {
                    r.TimeEnded = timeEnded;
                });

        public Task<IEnumerable<AppRequest>> Requests() => factory.Requests().RetrieveBySession(this);

        public Task<IEnumerable<AppRequest>> MostRecentRequests(int howMany)
            => factory.Requests().RetrieveMostRecent(this, howMany);

        public override string ToString() => $"{nameof(AppSession)} {ID.Value}";
    }
}
