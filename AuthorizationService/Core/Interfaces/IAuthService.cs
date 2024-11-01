using AuthorizationService.Shared.DTOs;

namespace AuthorizationService.Core.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDTO registerDTO);
        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<string?> ValidateTokenAsync(ValidateTokenDTO validateTokenDTO);
    }
}
