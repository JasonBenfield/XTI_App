using Microsoft.Extensions.Hosting;

namespace XTI_Core.Extensions
{
    public static class Extensions
    {
        public static bool IsTest(this IHostEnvironment env) => env.IsEnvironment("Test");
        public static bool IsDevOrTest(this IHostEnvironment env) => env != null && (env.IsDevelopment() || env.IsTest());

    }
}
