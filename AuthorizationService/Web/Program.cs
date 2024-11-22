using AuthorizationService.Core.Interfaces;
using AuthorizationService.Core.Entities;
using AuthorizationService.Infrastructure.Data;
using AuthorizationService.Infrastructure.Repositories;
using AuthorizationService.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthorizationService.Web.GRPC;
using AuthorizationService.Core.Profiles;
using Common;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        RabbitMQService.Configure(builder.Configuration);

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

        builder.Services.AddDbContext<DataBaseContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DataBaseConnection")));

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ITokenSettingsRepository, TokenSettingsRepository>();

        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<ISettingsService, SettingsService>();

        builder.Services.AddAutoMapper(typeof(AuthProfile));

        builder.Services.AddGrpc();

        var app = builder.Build();

        app.UseHttpsRedirection();

        if (!isTesting)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        app.MapGrpcService<GRPCService>();

        app.Use(async (context, next) =>
        {
            await next();
        });

        app.Run();
    }
}