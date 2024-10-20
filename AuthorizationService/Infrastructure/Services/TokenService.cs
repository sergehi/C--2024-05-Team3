using AuthorizationService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using AuthorizationService.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Grpc.Core;

namespace AuthorizationService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ISettingsService _settingsService;
        private readonly UserManager<User> _userManager;

        public TokenService(IConfiguration configuration, ISettingsService settingsService, UserManager<User> userManager)
        {
            _configuration = configuration;
            _settingsService = settingsService;
            _userManager = userManager;
        }

        public async Task<string> GenerateTokensAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Generate tokens failed: {userName}."));
            }

            var tokenSettings = await _settingsService.GetTokenSettingsAsync();
            var accessToken = await GenerateAccessTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(tokenSettings.RefreshTokenExpiryDays);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new RpcException(new Status(StatusCode.Aborted, $"Generate tokens failed: {errors}."));
            }

            return accessToken;
        }

        public async Task<string> GenerateAccessTokenAsync(User user)
        {
            if (string.IsNullOrEmpty(user.UserName))
                throw new RpcException(new Status(StatusCode.Internal, $"Generate access token failed: username is null."));

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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
            var tokenSettings = await _settingsService.GetTokenSettingsAsync();
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

        public string GenerateRefreshToken()
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

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            return principal;
        }
    }
}
