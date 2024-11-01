using AuthorizationService.Core.Interfaces;
using AuthorizationService.Core.Entities;
using AuthorizationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationService.Infrastructure.Repositories
{
    public class TokenSettingsRepository : ITokenSettingsRepository
    {
        private readonly DataBaseContext _dataBaseContext;

        public TokenSettingsRepository(DataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }

        public async Task<TokenSettings?> FindAsync()
        {
            return await _dataBaseContext.TokensSettings.OrderBy(x => x.Id).FirstOrDefaultAsync();
        }
    }
}