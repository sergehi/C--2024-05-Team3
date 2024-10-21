using AuthorizationService.Core.Interfaces;
using AuthorizationService.Core.Entities;
using AuthorizationService.Infrastructure.Data;
using AuthorizationService.Infrastructure.Repositories;
using AuthorizationService.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        var isTesting = builder.Environment.EnvironmentName == "IntegrationTesting";

        if (!isTesting)
        {
            builder.Services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                    });

            builder.Services.AddAuthorization();
        }

        builder.Services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<ISettingsService, SettingsService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();

        builder.Services.AddGrpc();

        var app = builder.Build();

        app.UseHttpsRedirection();

        if (!isTesting)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        app.MapGrpcService<AuthorizationService.Web.gRPC.AuthGrpcService>();

        app.Use(async (context, next) =>
        {
            await next();
        });

        app.Run();
    }
}