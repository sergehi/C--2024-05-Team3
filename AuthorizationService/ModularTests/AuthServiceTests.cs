using AuthorizationService.Core.Interfaces;
using AuthorizationService.Web.GRPC;
using Grpc.Core;
using Moq;
using System.Threading.Tasks;
using Xunit;
using System;
using Grpc.Core.Testing;
using AuthorizationService.Shared.Protos;

namespace AuthorizationService.ModularTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authService = new AuthService(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_Returns_Valid_UserResponse_With_Tokens()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Username = "testuserregister",
                Password = "Password123!",
            };

            _authServiceMock.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>()))
                .ReturnsAsync();

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

            var result = await _authService.Register(registerRequest, context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Success);
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

            var result = await _authService.Login(loginRequest, context);

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
        }

        [Fact]
        public async Task ValidateToken_Returns_Valid_AccessToken()
        {
            // Arrange
            var validateTokenRequest = new ValidateTokenRequest
            {
                AccessToken = "testAccessToken"
            };

            var validateTokenResponse = new ValidateTokenResponse
            {
                AccessToken = "newTestAccessToken",
            };

            _authServiceMock.Setup(x => x.ValidateTokenAsync(It.IsAny<string>()))
                            .ReturnsAsync(validateTokenResponse.AccessToken);

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

            var result = await _authService.ValidateToken(validateTokenRequest, context);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.AccessToken == null);
        }
    }
}
