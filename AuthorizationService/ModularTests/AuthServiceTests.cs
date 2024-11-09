using AuthorizationService.Core.Interfaces;
using Grpc.Core;
using Moq;
using Grpc.Core.Testing;
using AuthorizationService.Shared.Protos;
using AuthorizationService.Web.GRPC;
using AuthorizationService.Shared.DTOs;

namespace AuthorizationService.ModularTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly GRPCService _gRPcService;

        public AuthServiceTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _gRPcService = new GRPCService(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_Returns_Valid_UserResponse_With_Tokens()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Username = "testuserregister",
                Password = "Password123!"
            };

            _authServiceMock.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>()))
                .Returns(Task.CompletedTask);

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

            var result = await _gRPcService.Register(registerRequest, context);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Login_Returns_Valid_UserResponse_With_Tokens()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuserlogin",
                Password = "Password123!"
            };

            var loginResponse = new LoginResponse
            {
                AccessToken = "testaccesstoken",
                RefreshToken = "testrefreshtoken",
                Id = Guid.NewGuid().ToString(),
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

            var result = await _gRPcService.Login(loginRequest, context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testaccesstoken", result.AccessToken);
            Assert.Equal("testrefreshtoken", result.RefreshToken);
            Assert.NotNull(result.Id);
        }

        [Fact]
        public async Task ValidateToken_Returns_Valid_AccessToken()
        {
            // Arrange
            var validateTokenRequest = new ValidateTokenRequest
            {
                AccessToken = "testAccessToken"
            };

            _authServiceMock.Setup(x => x.ValidateTokenAsync(It.IsAny<ValidateTokenRequest>()))
                .Returns(Task.CompletedTask);

            // Act
            var context = TestServerCallContext.Create(
                method: "ValidateToken",
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

            var result = await _gRPcService.ValidateToken(validateTokenRequest, context);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ExtendToken_Returns_Valid_NewAccessToken()
        {
            // Arrange
            var extendTokenRequest = new ExtendTokenRequest
            {
                AccessToken = "oldAccessToken",
                RefreshToken = "validRefreshToken"
            };

            var extendTokenResponse = new ExtendTokenResponse
            {
                AccessToken = "newAccessToken",
            };

            // Настраиваем мок IAuthService для возврата нового access token
            _authServiceMock.Setup(x => x.ExtendTokenAsync(It.IsAny<ExtendTokenRequest>()))
                .ReturnsAsync(extendTokenResponse);

            // Act
            var context = TestServerCallContext.Create(
                method: "ExtendToken",
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

            var result = await _gRPcService.ExtendToken(extendTokenRequest, context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(extendTokenResponse.AccessToken, result.AccessToken);
        }
    }
}
