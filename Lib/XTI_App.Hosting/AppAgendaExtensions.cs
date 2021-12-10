using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace XTI_App.Hosting;

public static class AppAgendaExtensions
{
    public static void AddAppAgenda(this IServiceCollection services, IConfiguration configuration, Action<IServiceProvider, AppAgendaBuilder> build)
    {
        services.Configure<AppAgendaOptions>(configuration.GetSection(AppAgendaOptions.AppAgenda));
        var serviceDescriptors = services
            .Where(s => s.ServiceType == typeof(AppAgendaBuilder))
            .ToArray();
        foreach (var serviceDescriptor in serviceDescriptors)
        {
            services.Remove(serviceDescriptor);
        }
        services.AddScoped(sp =>
        {
            var builder = new AppAgendaBuilder(sp);
            build(sp, builder);
            var options = sp.GetRequiredService<IOptions<AppAgendaOptions>>().Value;
            builder.ApplyOptions(options);
            return builder.Build();
        });
    }
}