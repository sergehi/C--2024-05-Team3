using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using TaskTracker.Gateway.Configutions;
using TestGrpcService1; // Подключаем пространство имен gRPC для Service1

namespace TaskTracker.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var grpcConfig = builder.Configuration.GetSection("GrpcServices").Get<GrpcServiceConfig>();

            //gRpC services registration
            builder.Services.AddGrpcClient<Greeter.GreeterClient>(options =>
            {
                options.Address = new Uri(grpcConfig.GreeterServiceUrl);
            });


            var swaggerServices = builder.Configuration.GetSection("SwaggerServices").Get<List<SwaggerServiceConfig>>();
            builder.Services.AddSwaggerGen(c =>
            {
                foreach (var service in swaggerServices)
                {
                    c.SwaggerDoc(service.Name, new OpenApiInfo { Title = service.Title, Version = service.Version });
                }

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (apiDesc.GroupName == null) return false;
                    return apiDesc.GroupName == docName;
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    foreach (var service in swaggerServices)
                    {
                        c.SwaggerEndpoint(service.Endpoint, service.Title);
                    }
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
