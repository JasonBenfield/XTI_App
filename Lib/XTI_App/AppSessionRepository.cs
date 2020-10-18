using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                UserID = user.ID,
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
