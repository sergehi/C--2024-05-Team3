using ChatService.Infrastructure.EntityFramework;
using ChatService.API.Settings;
using ChatService.Infrastructure.Repositories.Implementations;
using ChatService.Services.Repositories.Abstractions;

namespace ChatService.API;

public static class Registrator
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationSettings = configuration.Get<ApplicationSettings>();
        services.AddSingleton(applicationSettings)
                .AddSingleton((IConfigurationRoot)configuration)
                .ConfigureContext(applicationSettings.ConnectionString)
                .InstallRepositories();

        return services;
    }
    
    private static IServiceCollection InstallRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IConversationRepository, ConversationRepository>()
            .AddTransient<IMediaFileRepository, MediaFileRepository>()
            .AddTransient<IMessageRepository, MessageRepository>()
            .AddTransient<IReactionRepository, ReactionRepository>();
        return serviceCollection;
    }
}