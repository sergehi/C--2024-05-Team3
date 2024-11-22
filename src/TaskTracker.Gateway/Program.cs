using AuthorizationService.Shared.Protos;
using ChatProto;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.OpenApi.Models;
using TaskTracker.Gateway.Configutions;
using TaskTracker.Gateway.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TaskTracker.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders(); // Очистить существующие провайдеры
            builder.Logging.AddConsole(); // Добавить логирование в консоль
            builder.Logging.AddDebug();   // Добавить логирование для отладки
            builder.Logging.AddFilter("Microsoft.AspNetCore.Server.Kestrel", LogLevel.Debug); // Логирование Kestrel
            builder.Logging.SetMinimumLevel(LogLevel.Debug); // Установить минимальный уровень логов
            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
                logging.RequestBodyLogLimit = 4096; // Лимит на тело запроса
                logging.ResponseBodyLogLimit = 4096; // Лимит на тело ответа
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureHttpsDefaults(httpsOptions =>
                {
                    httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:3000")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });

            var grpcConfig = builder.Configuration.GetSection("GrpcServices").Get<GrpcServiceConfig>();

            builder.Services.AddGrpcClient<Conversation.ConversationClient>(options =>
            {
                options.Address = new Uri(grpcConfig.ChatServiceUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                return handler;
            });

            builder.Services.AddGrpcClient<AuthProtoService.AuthProtoServiceClient>(options =>
            {
                options.Address = new Uri(grpcConfig.AuthorizationServiceUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                return handler;
            });

            builder.Services.AddGrpcClient<TasksServiceProto.TasksServiceProto.TasksServiceProtoClient>(options =>
            {
                options.Address = new Uri(grpcConfig.TasksServiceUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                return handler;
            });

            var swaggerServices = builder.Configuration.GetSection("SwaggerServices").Get<List<SwaggerServiceConfig>>();

            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введите JWT токен в формате 'Bearer {токен}'"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

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

            // Добавление SignalR
            builder.Services.AddSignalR();


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
            app.UseHttpLogging();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowSpecificOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            // Настройка маршрутов для SignalR Hub
            app.MapHub<SignalHub>("/notifyhub");

            app.Run();
        }
    }
}
