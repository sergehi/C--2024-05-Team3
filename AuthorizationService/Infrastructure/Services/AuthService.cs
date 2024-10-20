using AuthorizationService.Core.Entities;
using Microsoft.AspNetCore.Identity;
using AuthorizationService.Core.Interfaces;
using Microsoft.AspNetCore.Identity.UI;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ProtoContracts.Protos;

namespace AuthorizationService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ISettingsService _settingsService;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService,
            ISettingsService settingsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _settingsService = settingsService;
        }

        public async Task<bool> RegisterAsync(RegisterRequest registerRequest)
        {
            var user = new User
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                MiddleName = registerRequest.MiddleName
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new RpcException(new Status(StatusCode.Aborted, $"Registration failed: {errors}."));
            }

            return true;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.UserName);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Login failed: {loginRequest.UserName}."));
            }

            var signInresult = await _signInManager.PasswordSignInAsync(user.UserName!, loginRequest.Password, false, false);
            if (!signInresult.Succeeded)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Login failed: {loginRequest.Password}."));
            }

            var generateTokensResult = await _tokenService.GenerateTokensAsync(loginRequest.UserName);

            return new LoginResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Email = user.Email,
                AccessToken = generateTokensResult
            };
        }

        public async Task<string> RefreshTokensAsync(string accessToken)
        {
            var claimsPrincipal = _tokenService.ValidateToken(accessToken);

            var userId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Refresh tokens failed: NameIdentifier is null."));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Refresh tokens failed: {userId}."));
            }

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new RpcException(new Status(StatusCode.Cancelled, $"Refresh tokens failed: {user.RefreshTokenExpiryTime}."));
            }

            var newAccessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var tokenSettings = await _settingsService.GetTokenSettingsAsync();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(tokenSettings.RefreshTokenExpiryDays);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new RpcException(new Status(StatusCode.Aborted, $"Refresh tokens failed: {errors}."));
            }

            return newAccessToken;
        }
    }
}