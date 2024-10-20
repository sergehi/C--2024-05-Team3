using AuthorizationService.Core.Interfaces;
using AuthorizationService.Web.gRPC;
using Grpc.Core;
using System.Threading.Tasks;
using ProtoContracts.Protos;

namespace AuthorizationService.Web.gRPC
{
    public class AuthGrpcService : AuthService.AuthServiceBase
    {
        private readonly IAuthService _authService;

        public AuthGrpcService(IAuthService authService)
        {
            _authService = authService;
        }

        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            var result = await _authService.RegisterAsync(request);

            return new RegisterResponse
            {
                Success = result
            };
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.UserName))
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Login failed: username is null."));

            var result = await _authService.LoginAsync(request);

            return result;
        }

        public override async Task<RefreshTokensResponse> RefreshTokens(RefreshTokensRequest request, ServerCallContext context)
        {
            var newAccessToken = await _authService.RefreshTokensAsync(request.AccessToken);

            return new RefreshTokensResponse
            {
                AccessToken = newAccessToken,
            };
        }
    }
}
