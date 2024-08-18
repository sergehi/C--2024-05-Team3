using System.Reflection;
using AutoMapper;
using ChatService.API.Mappings;
using ChatService.Infrastructure.EntityFramework;
using ChatService.API.Settings;
using ChatService.Infrastructure.Repositories.Implementations;
using ChatService.Services.Abstractions;
using ChatService.Services.Implementations;
using ChatService.Services.Repositories.Abstractions;

namespace ChatService.API;

public static class Registrator
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationSettings = configuration.Get<ApplicationSettings>();
        services.AddSingleton(applicationSettings)
                .AddSingleton((IConfigurationRoot)configuration)
                .AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()))
                .ConfigureContext(applicationSettings.ConnectionString)
                .InstallRepositories()
                .InstallServices();

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
    
    private static IServiceCollection InstallServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IConversationService, ConversationService>();
        return serviceCollection;
    }
        
    private static MapperConfiguration GetMapperConfiguration()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ConversationMappingsProfile>();
            cfg.AddProfile<Services.Implementations.Mapping.ConversationMappingsProfile>(); ;
        });
        configuration.AssertConfigurationIsValid();
        return configuration;
    }
}