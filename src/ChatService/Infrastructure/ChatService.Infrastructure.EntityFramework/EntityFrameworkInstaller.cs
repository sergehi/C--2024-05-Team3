using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.Infrastructure.EntityFramework
{
    public static class EntityFrameworkInstaller
    {
        public static IServiceCollection ConfigureContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<DatabaseContext>(optionBuilder
                => optionBuilder
                    .UseNpgsql(connectionString)
                );
            
            return services;
        }
    }
}
