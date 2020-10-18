using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace XTI_App.DB
{
    public sealed class AppDbBackup
    {
        private readonly AppDbContext appDbContext;

        public AppDbBackup(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public Task Run(string environmentName, string backupFilePath)
        {
            var dbName = new AppDbName(environmentName).Value;
            FormattableString commandText =
                $"BACKUP DATABASE {dbName} TO DISK = {backupFilePath}";
            return appDbContext.Database.ExecuteSqlInterpolatedAsync(commandText);
        }
    }
}
