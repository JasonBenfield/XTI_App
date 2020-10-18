using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace XTI_App.EF
{
    public sealed class EfAppFactory : AppFactory
    {
        public EfAppFactory(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
            unitOfWork = new UnitOfWork(appDbContext);
            dbSetLookup = new Dictionary<Type, object>
            {
                { typeof(AppSessionRecord), appDbContext.Sessions },
                { typeof(AppUserRecord), appDbContext.Users },
                { typeof(AppRequestRecord), appDbContext.Requests },
                { typeof(AppEventRecord), appDbContext.Events },
                { typeof(AppRecord), appDbContext.Apps },
                { typeof(AppVersionRecord), appDbContext.Versions },
                { typeof(AppRoleRecord), appDbContext.Roles },
                { typeof(AppUserRoleRecord), appDbContext.UserRoles }
            };
        }

        private readonly AppDbContext appDbContext;
        private readonly UnitOfWork unitOfWork;

        private readonly Dictionary<Type, object> dbSetLookup;

        protected override DataRepository<T> CreateDataRepository<T>()
            where T : class =>
                new EfDataRepository<T>(unitOfWork, appDbContext, (DbSet<T>)dbSetLookup[typeof(T)]);
    }
}
