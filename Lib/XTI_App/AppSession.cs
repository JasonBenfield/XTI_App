using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XTI_App.Entities;
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
        }

        public int ID { get => record.ID; }

        public bool HasStarted() => new Timestamp(record.TimeStarted).IsValid();
        public bool HasEnded() => new Timestamp(record.TimeEnded).IsValid();

        public Task<AppUser> User() => factory.Users().User(record.UserID);

        public Task<AppRequest> LogRequest(string requestKey, IAppVersion version, string path, DateTime timeRequested)
        {
            var requestRepo = factory.Requests();
            return requestRepo.Add(this, requestKey, version, path, timeRequested);
        }

        public Task Authenticate(IAppUser user)
        {
            return repo.Update(record, r =>
            {
                r.UserID = user.ID;
            });
        }

        public Task End(DateTime timeEnded)
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

        public override string ToString() => $"{nameof(AppSession)} {ID}";
    }
}
