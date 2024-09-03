using AutoMapper;
using Logger.DataAccess.EntityFramework;
using LoggerService.Mapping;
using Microsoft.EntityFrameworkCore;
using BLL_Abstr = Logger.BusinessLogic.Services.Abstractions;
using BLL_Impl = Logger.BusinessLogic.Services.Implementations;
using DAL_Abstr = Logger.DataAccess.Repositories.Abstractions;
using DAL_Impl = Logger.DataAccess.Repositories.Implementations;
using RabbitMQ.Client;

namespace LoggerService
{
    /// <summary>
    /// Регистратор сервисов
    /// </summary>
    public static class RegistrarServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Зарегистрируйте DbContext
            services.AddDbContext<DbContext, LoggerContext>(options => options.UseNpgsql(configuration["ConnectionStrings:LoggerDB"]));
            // Зарегистрируйте репозитории
            services.InstallRepositories();
            // Зарегистрируйте службы
            services.InstallServices();
            // Зарегистрируйте IMapper
            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));
            // Зарегистрируйте IConnection для RabbitMQ
            services.AddSingleton(RabbitMQConnection(configuration));
            return services;
        }
        private static IServiceCollection InstallRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<DAL_Abstr.ILogRepository, DAL_Impl.LogRepository>();
            return serviceCollection;
        }
        private static IServiceCollection InstallServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<BLL_Abstr.ILogService, BLL_Impl.LogService>();
            return serviceCollection;
        }
        private static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LogMappingsProfile>();
                cfg.AddProfile<BLL_Impl.Mapping.LogMappingsProfile>();
            });
            configuration.AssertConfigurationIsValid();
            return configuration;
        }
        private static IConnection RabbitMQConnection(IConfiguration configuration)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = configuration["RmqSettings:Host"],
                VirtualHost = configuration["RmqSettings:VHost"],
                UserName = configuration["RmqSettings:Login"],
                Password = configuration["RmqSettings:Password"]
            };
            return factory.CreateConnection();
        }
    }
}
