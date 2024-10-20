using System.Security.Claims;
using AuthorizationService.Core.Entities;

namespace AuthorizationService.Core.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal ValidateToken(string token);
        Task<string> GenerateTokensAsync(string userName);
    }
}
