using System.Security.Claims;

namespace AuthorizationService.Core.Interfaces
{
    public interface ITokenService
    {
        ClaimsPrincipal ValidateToken(string token);
        Task<string> GenerateTokensAsync(string username);
        bool IsTokenExpired(string accessToken);
        Task<string> GenerateAccessTokenAsync(Guid userId);
    }
}
