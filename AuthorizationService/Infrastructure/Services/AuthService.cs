using AuthorizationService.Core.Entities;
using Microsoft.AspNetCore.Identity;
using AuthorizationService.Core.Interfaces;
using Grpc.Core;
using AuthorizationService.Shared.DTOs;
using AutoMapper;
using System.Security.Claims;

namespace AuthorizationService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(IMapper mapper, IUserRepository userRepository, IPasswordHasher<User> passwordHasher, ITokenService tokenService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task RegisterAsync(RegisterDTO registerDTO)
        {
            try
            {
                var existingUser = await _userRepository.FindByUsernameAsync(registerDTO.Username);
                if (existingUser != null)
                {
                    throw new RpcException(new Status(StatusCode.AlreadyExists, "User already exists."));
                }

                User user = _mapper.Map<User>(registerDTO);

                user.PasswordHash = _passwordHasher.HashPassword(user, registerDTO.Password);

                await _userRepository.CreateAsync(user);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Registration failed: {ex.Message}"));
            }
        }

        public async Task<TokensDTO> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                var existingUser = await _userRepository.FindByUsernameAsync(loginDTO.Username);
                if (existingUser == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "The user was not found."));
                }

                var verificationResult = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, loginDTO.Password);
                if (verificationResult != PasswordVerificationResult.Success)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid password."));
                }

                return await _tokenService.GenerateTokensAsync(existingUser.Username);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Login failed: {ex.Message}"));
            }
        }

        public async Task ValidateTokenAsync(ValidateTokenDTO validateTokenDTO)
        {
            try
            {
                var claimsPrincipal = _tokenService.ValidateToken(validateTokenDTO.AccessToken);

                var userId = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, $"Access token doesn't contain a user Id: NameIdentifier is null."));
                }

                var user = await _userRepository.FindByIdAsync(new Guid(userId));
                if (user == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"User not found: {userId}."));
                }

                if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    throw new RpcException(new Status(StatusCode.Cancelled, $"Refresh token expired: {user.RefreshTokenExpiryTime}."));
                }

                if (user.AccessToken != validateTokenDTO.AccessToken)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, $"Wrong access token."));
                }
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Validate token failed: {ex.Message}"));
            }
        }

        public async Task<string> ExtendTokenAsync(TokensDTO tokensDTO)
        {
            try
            {
                var claimsPrincipal = _tokenService.ValidateToken(tokensDTO.AccessToken);

                var userId = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, $"Access token doesn't contain a user Id: NameIdentifier is null."));
                }

                var user = await _userRepository.FindByIdAsync(new Guid(userId));
                if (user == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"User not found: {userId}."));
                }

                if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    throw new RpcException(new Status(StatusCode.Cancelled, $"Refresh token expired: {user.RefreshTokenExpiryTime}."));
                }

                if (user.RefreshToken != tokensDTO.RefreshToken)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, $"Wrong refresh token."));
                }

                if (user.AccessToken != tokensDTO.AccessToken)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, $"Wrong access token."));
                }

                return await _tokenService.GenerateAccessTokenAsync(user.Id);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Extend token failed: {ex.Message}"));
            }
        }
    }
}