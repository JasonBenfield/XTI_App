using Microsoft.Extensions.Options;

namespace XTI_App.EF
{
    public sealed class AppConnectionString : XtiConnectionString
    {
        public AppConnectionString(IOptions<DbOptions> options, string envName)
            : this(options.Value, envName)
        {
        }

        public AppConnectionString(DbOptions options, string envName)
            : base(options, new AppDbName(envName))
        {
        }
    }
}
