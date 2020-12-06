using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainDB.Entities;
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

        public async Task<AppSession> Session(int id)
        {
            var record = await repo.Retrieve().FirstOrDefaultAsync(s => s.ID == id);
            return factory.Session(record);
        }

        public async Task<AppSession> Session(string sessionKey)
        {
            var record = await repo.Retrieve().FirstOrDefaultAsync(s => s.SessionKey == sessionKey);
            return factory.Session(record);
        }

        public async Task<AppSession> DefaultSession(DateTime minTimeStarted)
        {
            var sessionRecord = await repo.Retrieve()
                .FirstOrDefaultAsync(r => r.RequesterKey == "default" && r.TimeStarted >= minTimeStarted.Date);
            return factory.Session(sessionRecord);
        }

        public async Task<AppSession> CreateDefaultSession(DateTime timeStarted)
        {
            var user = await factory.Users().User(AppUserName.Anon);
            var session = await Create
            (
                new GeneratedKey().Value(),
                user,
                timeStarted,
                "default",
                "",
                ""
            );
            return session;
        }

        public async Task<IEnumerable<AppSession>> SessionsByTimeRange(DateTime startDate, DateTime endDate)
        {
            var records = await repo.Retrieve()
                .Where(s => s.TimeStarted >= startDate && s.TimeStarted < endDate)
                .ToArrayAsync();
            return records.Select(s => factory.Session(s));
        }

        public async Task<AppSession> Create(string sessionKey, IAppUser user, DateTime timeStarted, string requesterKey, string userAgent, string remoteAddress)
        {
            var record = new AppSessionRecord
            {
                SessionKey = sessionKey,
                UserID = user.ID.Value,
                TimeStarted = timeStarted,
                RequesterKey = requesterKey ?? "",
                TimeEnded = Timestamp.MaxValue.Value,
                UserAgent = userAgent ?? "",
                RemoteAddress = remoteAddress ?? ""
            };
            await repo.Create(record);
            return factory.Session(record);
        }
    }
}
