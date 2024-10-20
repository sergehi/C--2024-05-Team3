using System.Threading.Tasks;
using ProtoContracts.Protos;

namespace AuthorizationService.Core.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequest registerRequest);
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task<string> RefreshTokensAsync(string accessToken);
    }
}
