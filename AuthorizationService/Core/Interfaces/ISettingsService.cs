using AuthorizationService.Core.Entities;

namespace AuthorizationService.Core.Interfaces
{
    public interface ISettingsService
    {
        Task<TokenSettings> GetTokenSettingsAsync();
    }
}