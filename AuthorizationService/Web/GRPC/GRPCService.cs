using AuthorizationService.Core.Interfaces;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using AuthorizationService.Shared.Protos;

namespace AuthorizationService.Web.GRPC
{
    public class GRPCService : AuthProtoService.AuthProtoServiceBase
    {
        public readonly IAuthService _authService;

        public GRPCService(IAuthService authService)
        {
            _authService = authService;
        }

        public override async Task<Empty> Register(RegisterRequest request, ServerCallContext context)
        {
            await _authService.RegisterAsync(request);

            return new Empty();
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            return await _authService.LoginAsync(request);
        }

        public override async Task<Empty> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
        {
            await _authService.ValidateTokenAsync(request);

            return new Empty();
        }

        public override async Task<ExtendTokenResponse> ExtendToken(ExtendTokenRequest request, ServerCallContext context)
        {
            return await _authService.ExtendTokenAsync(request);
        }
    }
}
