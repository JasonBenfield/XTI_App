using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using XTI_App.DB;
using XTI_App.Entities;
using XTI_Core;
using XTI_Core.EF;

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
                { typeof(AppUserRoleRecord), appDbContext.UserRoles },
                { typeof(ResourceGroupRecord), appDbContext.ResourceGroups },
                { typeof(ResourceRecord), appDbContext.Resources },
                { typeof(ModifierCategoryRecord), appDbContext.ModifierCategories },
                { typeof(ModifierCategoryAdminRecord), appDbContext.ModifierCategoryAdmins },
                { typeof(ModifierRecord), appDbContext.Modifiers },
                { typeof(AppUserModifierRecord), appDbContext.UserModifiers }
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
