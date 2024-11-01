using AuthorizationService.Core.Interfaces;
using AuthorizationService.Core.Entities;
using Grpc.Core;

namespace AuthorizationService.Infrastructure.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ITokenSettingsRepository _tokenSettingsRepository;

        public SettingsService(ITokenSettingsRepository tokenSettingsRepository)
        {
            _tokenSettingsRepository = tokenSettingsRepository;
        }

        public async Task<TokenSettings> GetTokenSettingsAsync()
        {
            try
            {
                var tokenSettings = await _tokenSettingsRepository.FindAsync();
                if (tokenSettings == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Get token settings failed."));
                }

                return tokenSettings;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Generate tokens failed: {ex.Message}"));
            }
        }
    }
}