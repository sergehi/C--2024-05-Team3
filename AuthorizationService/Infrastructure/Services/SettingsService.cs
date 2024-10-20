using AuthorizationService.Core.Interfaces;
using AuthorizationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Grpc.Core;

namespace AuthorizationService.Infrastructure.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly AuthDbContext _context;

        public SettingsService(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<TokenSettings> GetTokenSettingsAsync()
        {
            var tokenSettings = await _context.TokenSettings.OrderBy(x => x.Id).FirstOrDefaultAsync();
            if (tokenSettings == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Get token settings failed."));
            }
            return tokenSettings;
        }
    }
}