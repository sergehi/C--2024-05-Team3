using AuthorizationService.Core.Interfaces;
using Grpc.Core;
using Moq;
using System.Threading.Tasks;
using Xunit;
using System;
using Grpc.Core.Testing;
using AuthorizationService.Shared.Protos;
using AutoMapper;
using AuthorizationService.Web.GRPC;
using AuthorizationService.Shared.DTOs;

namespace AuthorizationService.ModularTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly GRPCService _gRPcService;
        private readonly Mock<IMapper> _mapperMock;

        public AuthServiceTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _mapperMock = new Mock<IMapper>();
            _gRPcService = new GRPCService(_mapperMock.Object, _authServiceMock.Object);
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

            _authServiceMock.Setup(x => x.RegisterAsync(It.IsAny<RegisterDTO>()))
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<RegisterDTO>(It.IsAny<RegisterRequest>()))
                .Returns(new RegisterDTO
                {
                    Username = "testuserregister",
                    Password = "Password123!"
                });

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

            var tokensDTO = new TokensDTO
            {
                AccessToken = "testaccesstoken",
                RefreshToken = "testrefreshtoken"
            };

            _authServiceMock.Setup(x => x.LoginAsync(It.IsAny<LoginDTO>()))
                .ReturnsAsync(tokensDTO);

            _mapperMock.Setup(m => m.Map<LoginDTO>(It.IsAny<LoginRequest>()))
                .Returns(new LoginDTO
                {
                    Username = "testuserlogin",
                    Password = "Password123!"
                });

            _mapperMock.Setup(m => m.Map<LoginResponse>(It.IsAny<TokensDTO>()))
                .Returns(new LoginResponse
                {
                    AccessToken = "testaccesstoken",
                    RefreshToken = "testrefreshtoken"
                });

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
        }

        [Fact]
        public async Task ValidateToken_Returns_Valid_AccessToken()
        {
            // Arrange
            var validateTokenRequest = new ValidateTokenRequest
            {
                AccessToken = "testAccessToken"
            };

            _authServiceMock.Setup(x => x.ValidateTokenAsync(It.IsAny<ValidateTokenDTO>()))
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<ValidateTokenDTO>(It.IsAny<ValidateTokenRequest>()))
                .Returns(new ValidateTokenDTO
                {
                    AccessToken = "testAccessToken"
                });

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

            var tokensDTO = new TokensDTO
            {
                AccessToken = "oldAccessToken",
                RefreshToken = "validRefreshToken"
            };

            var newAccessToken = "newAccessToken";

            // Настраиваем маппер для преобразования ExtendTokenRequest в TokensDTO
            _mapperMock.Setup(m => m.Map<TokensDTO>(It.IsAny<ExtendTokenRequest>()))
                .Returns(tokensDTO);

            // Настраиваем мок IAuthService для возврата нового access token
            _authServiceMock.Setup(x => x.ExtendTokenAsync(It.IsAny<TokensDTO>()))
                .ReturnsAsync(newAccessToken);

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
            Assert.Equal(newAccessToken, result.AccessToken);
        }
    }
}
