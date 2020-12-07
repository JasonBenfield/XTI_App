using MainDB.Extensions;
using Microsoft.Extensions.DependencyInjection;
using XTI_Core;
using XTI_Core.Fakes;

namespace XTI_App.Tests
{
    public static class Extensions
    {
        public static void AddServicesForTests(this IServiceCollection services)
        {
            services.AddAppDbContextForInMemory();
            services.AddSingleton<AppFactory>();
            services.AddSingleton<FakeClock>();
            services.AddSingleton<Clock>(sp => sp.GetService<FakeClock>());
        }
    }
}
