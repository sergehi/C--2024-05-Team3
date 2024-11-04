using AuthorizationService.Shared.DTOs;

namespace AuthorizationService.Core.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDTO registerDTO);
        Task<TokensDTO> LoginAsync(LoginDTO loginDTO);
        Task ValidateTokenAsync(ValidateTokenDTO validateTokenDTO);
        Task<string> ExtendTokenAsync(TokensDTO tokensDTO);
    }
}
