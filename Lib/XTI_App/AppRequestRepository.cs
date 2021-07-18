using MainDB.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class AppRequestRepository
    {
        private readonly AppFactory factory;

        internal AppRequestRepository(AppFactory factory)
        {
            this.factory = factory;
        }

        internal async Task<AppRequest> Add(AppSession session, string requestKey, Resource resource, Modifier modifier, string path, DateTimeOffset timeRequested)
        {
            var record = new AppRequestRecord
            {
                SessionID = session.ID.Value,
                RequestKey = requestKey,
                ResourceID = resource.ID.Value,
                ModifierID = modifier.ID.Value,
                Path = path ?? "",
                TimeStarted = timeRequested
            };
            await factory.DB.Requests.Create(record);
            return factory.Request(record);
        }

        public async Task<AppRequest> Request(string requestKey)
        {
            var requestRecord = await factory.DB.Requests.Retrieve()
                .FirstOrDefaultAsync(r => r.RequestKey == requestKey);
            return factory.Request(requestRecord);
        }

        internal Task<AppRequest[]> RetrieveBySession(AppSession session)
            => factory.DB.Requests
                .Retrieve()
                .Where(r => r.SessionID == session.ID.Value)
                .Select(r => factory.Request(r))
                .ToArrayAsync();

        internal Task<AppRequest[]> RetrieveMostRecent(AppSession session, int howMany)
        {
            return factory.DB.Requests
                .Retrieve()
                .Where(r => r.SessionID == session.ID.Value)
                .OrderByDescending(r => r.TimeStarted)
                .Take(howMany)
                .Select(r => factory.Request(r))
                .ToArrayAsync();
        }

        internal async Task<AppRequestExpandedModel[]> MostRecentForVersion(AppVersion version, int howMany)
        {
            var resources = factory.DB
                .Resources
                .Retrieve()
                .Join
                (
                    factory.DB
                        .ResourceGroups
                        .Retrieve()
                        .Where(rg => rg.VersionID == version.ID.Value),
                    res => res.GroupID,
                    rg => rg.ID,
                    (res, rg) => new ResourceWithGroupRecord
                    {
                        ResourceID = res.ID,
                        ActionName = res.Name,
                        GroupID = rg.ID,
                        GroupName = rg.Name,
                        ResultType = ResourceResultType.Values.Value(res.ResultType)
                    }
                );
            var requests = await requestsWithResources(howMany, resources);
            return requests;
        }

        internal async Task<AppRequestExpandedModel[]> MostRecentForResourceGroup(ResourceGroup group, int howMany)
        {
            var resources = factory.DB
                .Resources
                .Retrieve()
                .Join
                (
                    factory.DB
                        .ResourceGroups
                        .Retrieve()
                        .Where(rg => rg.ID == group.ID.Value),
                    res => res.GroupID,
                    rg => rg.ID,
                    (res, rg) => new ResourceWithGroupRecord
                    {
                        ResourceID = res.ID,
                        ActionName = res.Name,
                        GroupID = rg.ID,
                        GroupName = rg.Name,
                        ResultType = ResourceResultType.Values.Value(res.ResultType)
                    }
                );
            var requests = await requestsWithResources(howMany, resources);
            return requests;
        }

        internal async Task<AppRequestExpandedModel[]> MostRecentForResource(Resource resource, int howMany)
        {
            var resources = factory.DB
                .Resources
                .Retrieve()
                .Where(r => r.ID == resource.ID.Value)
                .Join
                (
                    factory.DB
                        .ResourceGroups
                        .Retrieve(),
                    res => res.GroupID,
                    rg => rg.ID,
                    (res, rg) => new ResourceWithGroupRecord
                    {
                        ResourceID = res.ID,
                        ActionName = res.Name,
                        GroupID = rg.ID,
                        GroupName = rg.Name,
                        ResultType = ResourceResultType.Values.Value(res.ResultType)
                    }
                );
            var requests = await requestsWithResources(howMany, resources);
            return requests;
        }

        private Task<AppRequestExpandedModel[]> requestsWithResources(int howMany, IQueryable<ResourceWithGroupRecord> resources)
        {
            return factory.DB
                .Requests
                .Retrieve()
                .Join
                (
                    resources,
                    req => req.ResourceID,
                    res => res.ResourceID,
                    (req, res) => new
                    {
                        ID = req.ID,
                        SessionID = req.SessionID,
                        GroupName = res.GroupName,
                        ActionName = res.ActionName,
                        TimeStarted = req.TimeStarted,
                        TimeEnded = req.TimeEnded
                    }
                )
                .Join
                (
                    factory.DB
                        .Sessions
                        .Retrieve()
                        .Join
                        (
                            factory.DB
                                .Users
                                .Retrieve(),
                            s => s.UserID,
                            u => u.ID,
                            (s, u) => new
                            {
                                SessionID = s.ID,
                                u.UserName
                            }
                        ),
                    req => req.SessionID,
                    s => s.SessionID,
                    (req, s) => new
                    {
                        ID = req.ID,
                        UserName = s.UserName,
                        GroupName = req.GroupName,
                        ActionName = req.ActionName,
                        TimeStarted = req.TimeStarted,
                        TimeEnded = req.TimeEnded
                    }
                )
                .OrderByDescending(r => r.TimeStarted)
                .Take(howMany)
                .Select
                (
                    r => new AppRequestExpandedModel
                    {
                        ID = r.ID,
                        UserName = r.UserName,
                        GroupName = new ResourceGroupName(r.GroupName).DisplayText,
                        ActionName = new ResourceName(r.ActionName).DisplayText,
                        TimeStarted = r.TimeStarted,
                        TimeEnded = r.TimeEnded
                    }
                )
                .ToArrayAsync();
        }

        private sealed class ResourceWithGroupRecord
        {
            public int ResourceID { get; set; }
            public string ActionName { get; set; }
            public int GroupID { get; set; }
            public string GroupName { get; set; }
            public ResourceResultType ResultType { get; set; }
        }

    }
}
