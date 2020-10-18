using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace XTI_Secrets.Fakes
{
    public static class Extensions
    {
        public static void AddFakeSecretCredentials(this IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddScoped<SecretCredentialsFactory>(sp =>
            {
                var dataProtector = sp.GetDataProtector(new[] { "XTI_Secrets" });
                return new FakeSecretCredentialsFactory(dataProtector);
            });
        }
    }
}
