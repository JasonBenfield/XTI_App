using Microsoft.Extensions.DependencyInjection;
using XTI_App.Api;
using XTI_Core.Extensions;

namespace XTI_App.Hosting;

public static class AppAgendaExtensions
{
    public static void AddAppAgenda(this IServiceCollection services, Action<IServiceProvider, AppAgendaBuilder> build)
    {
        services.AddConfigurationOptions<AppAgendaOptions>(AppAgendaOptions.AppAgenda);
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
            var apiFactory = sp.GetRequiredService<AppApiFactory>();
            var api = apiFactory.CreateForSuperUser();
            var schedules = api.ActionSchedules();
            builder.ApplyOptions(schedules);
            build(sp, builder);
            var options = sp.GetRequiredService<AppAgendaOptions>();
            builder.ApplyOptions(options);
            return builder.Build();
        });
    }
}