using AuthorizationService.Core.Interfaces;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using AutoMapper;
using AuthorizationService.Shared.DTOs;
using AuthorizationService.Shared.Protos;

namespace AuthorizationService.Web.GRPC
{
    public class GRPCService : AuthService.AuthServiceBase
    {
        public readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public GRPCService(IMapper mapper, IAuthService authService)
        {
            _authService = authService;
            _mapper = mapper;
        }

        public override async Task<Empty> Register(RegisterRequest request, ServerCallContext context)
        {
            RegisterDTO registerDTO = _mapper.Map<RegisterDTO>(request);

            await _authService.RegisterAsync(registerDTO);

            return new Empty();
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            LoginDTO loginDTO = _mapper.Map<LoginDTO>(request);

            string accessToken = await _authService.LoginAsync(loginDTO);

            return new LoginResponse
            {
                AccessToken = accessToken
            };
        }

        public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
        {
            ValidateTokenDTO validateTokenDTO = _mapper.Map<ValidateTokenDTO>(request);

            string? accessToken = await _authService.ValidateTokenAsync(validateTokenDTO);

            return new ValidateTokenResponse
            {
                AccessToken = accessToken ?? string.Empty,
            };
        }
    }
}
