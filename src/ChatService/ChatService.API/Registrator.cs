using ChatService.Infrastructure.EntityFramework;
using ChatService.API.Settings;

namespace ChatService.API;

public static class Registrator
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationSettings = configuration.Get<ApplicationSettings>();
        services.AddSingleton(applicationSettings)
                .AddSingleton((IConfigurationRoot)configuration)
                .ConfigureContext(applicationSettings.ConnectionString);

        return services;
    }
}