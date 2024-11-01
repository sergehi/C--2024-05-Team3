using AuthorizationService.Core.Interfaces;
using AuthorizationService.Core.Entities;
using Microsoft.Extensions.Configuration;
using Grpc.Core;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace AuthorizationService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ISettingsService _settingsService;
        private readonly IUserRepository _userRepository;

        public TokenService(IConfiguration configuration, ISettingsService settingsService, IUserRepository userRepository)
        {
            _configuration = configuration;
            _settingsService = settingsService;
            _userRepository = userRepository;
        }

        public async Task<string> GenerateTokensAsync(string username)
        {
            try
            {
                var user = await _userRepository.FindByUsernameAsync(username);
                if (user == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Generate tokens failed: {username}."));
                }

                var tokenSettings = await _settingsService.GetTokenSettingsAsync();
                var accessToken = await GenerateAccessTokenAsync(user.Id);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(tokenSettings.RefreshTokenExpiryDays);
                await _userRepository.UpdateAsync(user);

                return accessToken;
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

        public async Task<string> GenerateAccessTokenAsync(Guid userId)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            };

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Generate access token failed: Jwt:Key is null."));
            }

            var jwtIssuer = _configuration["Jwt:Issuer"];
            if (string.IsNullOrEmpty(jwtIssuer))
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Generate access token failed: Jwt:Issuer is null."));
            }

            var jwtAudience = _configuration["Jwt:Audience"];
            if (string.IsNullOrEmpty(jwtAudience))
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Generate access token failed: Jwt:Audience is null."));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            TokenSettings tokenSettings = await _settingsService.GetTokenSettingsAsync();
            var accessTokenExpiry = tokenSettings.AccessTokenExpiryMinutes;

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(accessTokenExpiry),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(key))
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Validate token failed: Jwt:Key is null."));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            return principal;
        }

        public bool IsTokenExpired(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.CanReadToken(accessToken))
            {
                var jwtToken = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;

                var expiration = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

                if (expiration != null)
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiration)).UtcDateTime;
                    return expirationTime <= DateTime.UtcNow;
                }
            }

            return true;
        }
    }
}
