using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppSessionRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppSessionRecord> repo;

        internal AppSessionRepository(AppFactory factory, DataRepository<AppSessionRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        public async Task<AppSession> Session(string sessionKey)
        {
            var record = await repo.Retrieve().FirstOrDefaultAsync(s => s.SessionKey == sessionKey);
            return factory.Session(record);
        }

        public async Task<AppSession> DefaultSession(DateTimeOffset minTimeStarted)
        {
            var sessionRecord = await repo.Retrieve()
                .FirstOrDefaultAsync(r => r.RequesterKey == "default" && r.TimeStarted >= minTimeStarted.Date);
            return factory.Session(sessionRecord);
        }

        public async Task<IEnumerable<AppSession>> ActiveSessions(TimeRange timeRange)
        {
            var records = await repo.Retrieve()
                .Where(s => s.TimeEnded == DateTimeOffset.MaxValue && s.TimeStarted >= timeRange.Start && s.TimeStarted <= timeRange.End)
                .ToArrayAsync();
            return records.Select(s => factory.Session(s));
        }

        public async Task<IEnumerable<AppSession>> SessionsByTimeRange(TimeRange timeRange)
        {
            var records = await repo.Retrieve()
                .Where(s => s.TimeStarted >= timeRange.Start && s.TimeStarted <= timeRange.End)
                .ToArrayAsync();
            return records.Select(s => factory.Session(s));
        }

        public async Task<AppSession> Create(string sessionKey, IAppUser user, DateTimeOffset timeStarted, string requesterKey, string userAgent, string remoteAddress)
        {
            var record = new AppSessionRecord
            {
                SessionKey = sessionKey,
                UserID = user.ID.Value,
                TimeStarted = timeStarted,
                RequesterKey = requesterKey ?? "",
                TimeEnded = DateTimeOffset.MaxValue,
                UserAgent = userAgent ?? "",
                RemoteAddress = remoteAddress ?? ""
            };
            await repo.Create(record);
            return factory.Session(record);
        }
    }
}
