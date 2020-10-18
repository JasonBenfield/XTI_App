using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace XTI_App.EF
{
    public sealed class AppDbReset
    {
        private readonly AppDbContext appDbContext;

        public AppDbReset(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task Run()
        {
            await appDbContext.Database.ExecuteSqlRawAsync("delete from Events");
            await appDbContext.Database.ExecuteSqlRawAsync("delete from Requests");
            await appDbContext.Database.ExecuteSqlRawAsync("delete from Versions");
            await appDbContext.Database.ExecuteSqlRawAsync("delete from Sessions");
            await appDbContext.Database.ExecuteSqlRawAsync("delete from Roles");
            await appDbContext.Database.ExecuteSqlRawAsync("delete from UserRoles");
            await appDbContext.Database.ExecuteSqlRawAsync("delete from Users");
            await appDbContext.Database.ExecuteSqlRawAsync("delete from Apps");
        }
    }
}
