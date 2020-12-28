using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_App;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppSession
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppSessionRecord> repo;
        private readonly AppSessionRecord record;

        internal AppSession(AppFactory factory, DataRepository<AppSessionRecord> repo, AppSessionRecord record)
        {
            this.factory = factory;
            this.repo = repo;
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
            AppVersion version,
            Resource resource,
            Modifier modifier,
            string path,
            DateTimeOffset timeRequested
        )
        {
            var requestRepo = factory.Requests();
            return requestRepo.Add(this, requestKey, version, resource, modifier, path, timeRequested);
        }

        public Task Edit(AppUser user, DateTimeOffset timeStarted, string requesterKey, string userAgent, string remoteAddress)
            => repo.Update
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
        {
            return repo.Update(record, r =>
            {
                r.UserID = user.ID.Value;
            });
        }

        public Task End(DateTimeOffset timeEnded)
        {
            return repo.Update(record, r =>
            {
                r.TimeEnded = timeEnded;
            });
        }

        public Task<IEnumerable<AppRequest>> Requests()
        {
            var requestRepo = factory.Requests();
            return requestRepo.RetrieveBySession(this);
        }

        public override string ToString() => $"{nameof(AppSession)} {ID.Value}";
    }
}
