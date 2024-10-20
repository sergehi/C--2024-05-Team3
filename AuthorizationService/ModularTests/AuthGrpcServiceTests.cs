using AuthorizationService.Core.Interfaces;
using AuthorizationService.Web.gRPC;
using Grpc.Core;
using Moq;
using System.Threading.Tasks;
using Xunit;
using System;
using Grpc.Core.Testing;
using ProtoContracts.Protos;

namespace AuthorizationService.ModularTests
{
    public class AuthGrpcServiceTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthGrpcService _authGrpcService;

        public AuthGrpcServiceTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authGrpcService = new AuthGrpcService(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_Returns_Valid_UserResponse_With_Tokens()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                UserName = "testuserregister",
                Email = "testuserregister@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "Middle"
            };

            var registerResponse = new RegisterResponse
            {
                Success = true
            };

            _authServiceMock.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>()))
                .ReturnsAsync(registerResponse.Success);

            // Act
            var context = TestServerCallContext.Create(
                method: "Register",
                host: null,
                deadline: DateTime.UtcNow.AddMinutes(1),
                requestHeaders: new Metadata(),
                cancellationToken: CancellationToken.None,
                peer: null,
                authContext: null,
                contextPropagationToken: null,
                writeHeadersFunc: null,
                writeOptionsGetter: null,
                writeOptionsSetter: null
            );

            var result = await _authGrpcService.Register(registerRequest, context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(registerResponse.Success, result.Success);
        }

        [Fact]
        public async Task Login_Returns_Valid_UserResponse_With_Tokens()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                UserName = "testuserlogin",
                Password = "Password123!"
            };

            var loginResponse = new LoginResponse
            {
                Id = Guid.NewGuid().ToString(),
                Email = "testuserlogin@example.com",
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "Middle",
                AccessToken = "testaccesstoken"
            };

            _authServiceMock.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(loginResponse);

            // Act
            var context = TestServerCallContext.Create(
                method: "Login",
                host: null,
                deadline: DateTime.UtcNow.AddMinutes(1),
                requestHeaders: new Metadata(),
                cancellationToken: CancellationToken.None,
                peer: null,
                authContext: null,
                contextPropagationToken: null,
                writeHeadersFunc: null,
                writeOptionsGetter: null,
                writeOptionsSetter: null
            );

            var result = await _authGrpcService.Login(loginRequest, context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(loginResponse.Id, result.Id);
            Assert.Equal(loginResponse.Email, result.Email);
            Assert.Equal(loginResponse.FirstName, result.FirstName);
            Assert.Equal(loginResponse.LastName, result.LastName);
            Assert.Equal(loginResponse.MiddleName, result.MiddleName);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
        }

        [Fact]
        public async Task RefreshTokens_Returns_Valid_AccessToken()
        {
            // Arrange
            var refreshTokensRequest = new RefreshTokensRequest
            {
                AccessToken = "testAccessToken"
            };

            var refreshTokensResponse = new RefreshTokensResponse
            {
                AccessToken = "newTestAccessToken",
            };

            _authServiceMock.Setup(x => x.RefreshTokensAsync(It.IsAny<string>()))
                            .ReturnsAsync(refreshTokensResponse.AccessToken);

            // Act
            var context = TestServerCallContext.Create(
                method: "RefreshTokens",
                host: null,
                deadline: DateTime.UtcNow.AddMinutes(1),
                requestHeaders: new Metadata(),
                cancellationToken: CancellationToken.None,
                peer: null,
                authContext: null,
                contextPropagationToken: null,
                writeHeadersFunc: null,
                writeOptionsGetter: null,
                writeOptionsSetter: null
            );

            var result = await _authGrpcService.RefreshTokens(refreshTokensRequest, context);

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
        }
    }
}
