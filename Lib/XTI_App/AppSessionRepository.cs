using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppSessionRepository
    {
        private readonly AppFactory factory;

        internal AppSessionRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        public async Task<AppSession> Session(string sessionKey)
        {
            var record = await factory.DB
                .Sessions
                .Retrieve()
                .FirstOrDefaultAsync(s => s.SessionKey == sessionKey);
            return factory.Session(record);
        }

        public async Task<AppSession> DefaultSession(DateTimeOffset minTimeStarted)
        {
            var sessionRecord = await factory.DB
                .Sessions
                .Retrieve()
                .FirstOrDefaultAsync(r => r.RequesterKey == "default" && r.TimeStarted >= minTimeStarted.Date);
            return factory.Session(sessionRecord);
        }

        public Task<AppSession[]> ActiveSessions(TimeRange timeRange)
            => factory.DB
                .Sessions
                .Retrieve()
                .Where(s => s.TimeEnded == DateTimeOffset.MaxValue && s.TimeStarted >= timeRange.Start && s.TimeStarted <= timeRange.End)
                .Select(s => factory.Session(s))
                .ToArrayAsync();

        public Task<AppSession[]> SessionsByTimeRange(TimeRange timeRange)
            => factory.DB
                .Sessions
                .Retrieve()
                .Where(s => s.TimeStarted >= timeRange.Start && s.TimeStarted <= timeRange.End)
                .Select(s => factory.Session(s))
                .ToArrayAsync();

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
            await factory.DB.Sessions.Create(record);
            return factory.Session(record);
        }
    }
}
