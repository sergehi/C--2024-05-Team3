using System.Security.Claims;
using AuthorizationService.Shared.DTOs;

namespace AuthorizationService.Core.Interfaces
{
    public interface ITokenService
    {
        ClaimsPrincipal ValidateToken(string token);
        Task<TokensDTO> GenerateTokensAsync(string username);
        bool IsTokenExpired(string accessToken);
        Task<string> GenerateAccessTokenAsync(Guid userId);
    }
}
