using AutoMapper;
using Microsoft.EntityFrameworkCore;

using TasksService.DataAccess.EntityFramework;
using DalAbstr = TasksService.DataAccess.Repositories.Abstractions;
using DalImpls = TasksService.DataAccess.Repositories.Implementations;
using BllAbstr = TasksService.BusinessLogic.Services.Abstractions;
using BllImpls = TasksService.BusinessLogic.Services.Implementations;
using TasksService.Mapping;




namespace TasksService
{
    public static class ServiceCollectionExt
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Зарегистрируйте DbContext
            //services.AddDbContext<DbContext, TasksDbContext>(options => options.UseNpgsql(configuration["ConnectionStrings:TasksDb"]));
            services.AddDbContext<DbContext, TasksDbContext>();
            // Зарегистрируйте репозитории
            services.InstallRepositories();
            // Зарегистрируйте службы
            services.InstallServices();
            // Зарегистрируйте IMapper
            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));
            return services;
        }

        private static IServiceCollection InstallRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<DalAbstr.ITasksRepository, DalImpls.TemplatesRepository>();
            return serviceCollection;
        }

        private static IServiceCollection InstallServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<BllAbstr.ITasksService, BllImpls.TasksService>();
            return serviceCollection;
        }

        private static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TasksControllerMappingProfile>();
                cfg.AddProfile<BllImpls.Mapping.TasksMappingProfile>();
            });
            configuration.AssertConfigurationIsValid();
            return configuration;
        }


    }

}
