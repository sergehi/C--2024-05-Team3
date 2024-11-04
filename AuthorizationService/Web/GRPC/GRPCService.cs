using AuthorizationService.Core.Interfaces;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using AutoMapper;
using AuthorizationService.Shared.DTOs;
using AuthorizationService.Shared.Protos;

namespace AuthorizationService.Web.GRPC
{
    public class GRPCService : AuthProtoService.AuthProtoServiceBase
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

            TokensDTO tokensDTO = await _authService.LoginAsync(loginDTO);

            return _mapper.Map<LoginResponse>(tokensDTO);
        }

        public override async Task<Empty> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
        {
            ValidateTokenDTO validateTokenDTO = _mapper.Map<ValidateTokenDTO>(request);

            await _authService.ValidateTokenAsync(validateTokenDTO);

            return new Empty();
        }

        public override async Task<ExtendTokenResponse> ExtendToken(ExtendTokenRequest request, ServerCallContext context)
        {
            TokensDTO tokensDTO = _mapper.Map<TokensDTO>(request);

            string newAccessToken = await _authService.ExtendTokenAsync(tokensDTO);

            return new ExtendTokenResponse
            {
                AccessToken = newAccessToken
            };
        }
    }
}
