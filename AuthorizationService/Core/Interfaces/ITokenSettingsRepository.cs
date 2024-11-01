using AuthorizationService.Core.Entities;

namespace AuthorizationService.Core.Interfaces
{
    public interface ITokenSettingsRepository
    {
        Task<TokenSettings?> FindAsync();
    }
}