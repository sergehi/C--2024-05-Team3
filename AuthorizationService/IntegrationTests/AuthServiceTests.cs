using AuthorizationService.Core.Entities;
using AuthorizationService.Infrastructure.Data;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProtoContracts.Protos;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace AuthorizationService.IntegrationTests
{
    public class AuthServiceTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private AuthService.AuthServiceClient? _authServiceClient;
        private RegisterRequest? _registerRequest;
        private LoginRequest? _loginRequest;
        private RefreshTokensRequest? _refreshTokensRequest;

        public AuthServiceTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Environment", "IntegrationTesting");

                builder.ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();

                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { "Jwt:Issuer", "TestIssuerFromSecrets" },
                        { "Jwt:Audience", "TestAudienceFromSecrets" },
                        { "Jwt:Key", "02a8da25ef906ac80c8da5380bb50f2a2a6af676c098b1b6acf70b0d69d98fd554e079858068d98c15b4bbe7c61871c874de06f8d153d4422e126a079527a4d231914eb39279a85e4162afd715dd83a0b992809a478c66016c5e5676174ca25413270e66bf17cee0f7fdc1f0668f9e2098f0fc8ff36ad65c1a7e744c12ace4dbf6829f62ae766aecbc4e38850f9ba39c45d38e8d8a379f9b55e9c4ffb087b115712ee41ea0ed1c968d0ab976481b6ac7c23033b30ad5771bea697ab94cfcdb1286641efa946c4055ff7161e10c14feeb44047ccd399df3ebfb629f8ceff1e6280754de92af41afbac2f916d69b746d0a2985ea12cdd7cbab906d2878e6bbbfba" },
                    });
                });

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.FirstOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<AuthDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryAuthService");
                    });

                    var serviceProvider = services.BuildServiceProvider();
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<AuthDbContext>();

                        db.Database.EnsureCreated();

                        SeedTokenSettings(db);
                    }
                });
            });

            InitializeGrpcClient();
            CreateRegisterRequest();
        }

        [Fact]
        public void Configuration_Contains_JwtSettings()
        {
            using var scope = _factory.Services.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var key = configuration["Jwt:Key"];

            Assert.Equal("TestIssuerFromSecrets", issuer);
            Assert.Equal("TestAudienceFromSecrets", audience);
            Assert.Equal("02a8da25ef906ac80c8da5380bb50f2a2a6af676c098b1b6acf70b0d69d98fd554e079858068d98c15b4bbe7c61871c874de06f8d153d4422e126a079527a4d231914eb39279a85e4162afd715dd83a0b992809a478c66016c5e5676174ca25413270e66bf17cee0f7fdc1f0668f9e2098f0fc8ff36ad65c1a7e744c12ace4dbf6829f62ae766aecbc4e38850f9ba39c45d38e8d8a379f9b55e9c4ffb087b115712ee41ea0ed1c968d0ab976481b6ac7c23033b30ad5771bea697ab94cfcdb1286641efa946c4055ff7161e10c14feeb44047ccd399df3ebfb629f8ceff1e6280754de92af41afbac2f916d69b746d0a2985ea12cdd7cbab906d2878e6bbbfba",
                key);
        }

        private void SeedTokenSettings(AuthDbContext dbContext)
        {
            dbContext.TokenSettings.Add(new TokenSettings
            {
                AccessTokenExpiryMinutes = 30,
                RefreshTokenExpiryDays = 30
            });
            dbContext.SaveChanges();
        }

        private void InitializeGrpcClient()
        {
            var client = _factory.CreateDefaultClient();
            var baseAddress = client.BaseAddress ?? new Uri("https://localhost:5001");
            var grpcChannel = GrpcChannel.ForAddress(baseAddress, new GrpcChannelOptions { HttpClient = client });
            _authServiceClient = new AuthService.AuthServiceClient(grpcChannel);
        }

        private async Task<RegisterResponse> RegisterUserAsync()
        {
            if (_authServiceClient == null)
            {
                throw new InvalidOperationException("gRPC client is not initialized.");
            }
            var result = await _authServiceClient.RegisterAsync(_registerRequest);
            _loginRequest = new LoginRequest
            {
                UserName = _registerRequest!.UserName,
                Password = _registerRequest.Password,
            };
            return result;
        }

        private async Task<LoginResponse> LoginUserAsync()
        {
            if (_authServiceClient == null)
            {
                throw new InvalidOperationException("gRPC client is not initialized.");
            }
            var result = await _authServiceClient.LoginAsync(_loginRequest);
            _refreshTokensRequest = new RefreshTokensRequest
            {
                AccessToken = result.AccessToken,
            };
            return result;
        }

        private async Task<RefreshTokensResponse> RefreshTokensAsync()
        {
            if (_authServiceClient == null)
            {
                throw new InvalidOperationException("gRPC client is not initialized.");
            }
            return await _authServiceClient.RefreshTokensAsync(_refreshTokensRequest);
        }

        private void CreateRegisterRequest()
        {
            _registerRequest = new RegisterRequest
            {
                UserName = "testuserregister",
                Email = $"testuserregister@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "Middle"
            };
        }

        [Fact]
        public async Task Register_StoresUserInDatabase()
        {
            // Act
            var response = await RegisterUserAsync();

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "testuserregister");

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.NotNull(user);
            Assert.Equal(_registerRequest!.UserName, user.UserName);
            Assert.Equal(_registerRequest.Email, user.Email);
            Assert.Equal(_registerRequest.FirstName, user.FirstName);
            Assert.Equal(_registerRequest.LastName, user.LastName);
            Assert.Equal(_registerRequest.MiddleName, user.MiddleName);

            CleanupDatabase();
        }

        [Fact]
        public async Task Login_ReturnUserDataWithToken()
        {
            await RegisterUserAsync();

            // Act
            var response = await LoginUserAsync();

            // Assert
            Assert.NotNull(response);
            Assert.False(string.IsNullOrEmpty(response.AccessToken));
            Assert.False(string.IsNullOrEmpty(response.Id));
            Assert.Equal(_registerRequest!.Email, response.Email);
            Assert.Equal(_registerRequest.FirstName, response.FirstName);
            Assert.Equal(_registerRequest.LastName, response.LastName);
            Assert.Equal(_registerRequest.MiddleName, response.MiddleName);

            CleanupDatabase();
        }

        [Fact]
        public async Task RefreshTokens_ReturnsNewAccessToken()
        {
            await RegisterUserAsync();

            await LoginUserAsync();

            // Act
            var response = await _authServiceClient!.RefreshTokensAsync(_refreshTokensRequest);

            // Assert
            Assert.NotNull(response);
            Assert.False(string.IsNullOrEmpty(response.AccessToken));

            CleanupDatabase();
        }

        private void CleanupDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            db.Users.RemoveRange(db.Users);
            db.SaveChanges();
        }
    }
}
