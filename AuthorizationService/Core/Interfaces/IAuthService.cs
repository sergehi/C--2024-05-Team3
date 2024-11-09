using AuthorizationService.Shared.Protos;

namespace AuthorizationService.Core.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest registerRequest);
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task ValidateTokenAsync(ValidateTokenRequest validateTokenRequest);
        Task<ExtendTokenResponse> ExtendTokenAsync(ExtendTokenRequest extendTokenRequest);
    }
}
