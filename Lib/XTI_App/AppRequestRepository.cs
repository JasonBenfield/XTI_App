﻿using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppRequestRepository
    {
        private readonly AppFactory factory;
        private readonly DataRepository<AppRequestRecord> repo;

        internal AppRequestRepository(AppFactory factory, DataRepository<AppRequestRecord> repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        internal async Task<AppRequest> Add(AppSession session, string requestKey, AppVersion version, Resource resource, Modifier modifier, string path, DateTimeOffset timeRequested)
        {
            var record = new AppRequestRecord
            {
                SessionID = session.ID.Value,
                RequestKey = requestKey,
                VersionID = version.ID.Value,
                ResourceID = resource.ID.Value,
                ModifierID = modifier.ID.Value,
                Path = path ?? "",
                TimeStarted = timeRequested
            };
            await repo.Create(record);
            return factory.Request(record);
        }

        public async Task<AppRequest> Request(string requestKey)
        {
            var requestRecord = await repo.Retrieve()
                .FirstOrDefaultAsync(r => r.RequestKey == requestKey);
            return factory.Request(requestRecord);
        }

        internal async Task<IEnumerable<AppRequest>> RetrieveBySession(AppSession session)
        {
            var requests = await repo.Retrieve()
                .Where(r => r.SessionID == session.ID.Value)
                .ToArrayAsync();
            return requests.Select(r => factory.Request(r));
        }

        internal async Task<IEnumerable<AppRequest>> RetrieveMostRecent(AppSession session, int howMany)
        {
            var requests = await repo.Retrieve()
                .Where(r => r.SessionID == session.ID.Value)
                .OrderByDescending(r => r.TimeStarted)
                .Take(howMany)
                .ToArrayAsync();
            return requests.Select(r => factory.Request(r));
        }
    }
}
