using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using XTI_App.DB;

namespace XTI_App.Fakes
{
    public static class Extensions
    {
        public static void AddAppDbContextForInMemory(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .EnableSensitiveDataLogging();
            });
        }
    }
}
