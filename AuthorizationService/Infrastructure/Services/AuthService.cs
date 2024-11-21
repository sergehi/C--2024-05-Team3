using AuthorizationService.Core.Entities;
using Microsoft.AspNetCore.Identity;
using AuthorizationService.Core.Interfaces;
using Grpc.Core;
using AutoMapper;
using System.Security.Claims;
using AuthorizationService.Shared.Protos;
using AuthorizationService.Shared.DTOs;
using Common;
using Newtonsoft.Json;
using Common.Attributes;
using System.Diagnostics;

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

        public async Task RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                var existingUser = await _userRepository.FindByUsernameAsync(registerRequest.Username);
                if (existingUser != null)
                {
                    throw new RpcException(new Status(StatusCode.AlreadyExists, "User already exists."));
                }

                User user = _mapper.Map<User>(registerRequest);

                user.PasswordHash = _passwordHasher.HashPassword(user, registerRequest.Password);

                await _userRepository.CreateAsync(user);
                try
                {
                    RabbitMQService.SendToRabbit(user, LoggerService.ELogAction.LaCreate, user.Id.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
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

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var existingUser = await _userRepository.FindByUsernameAsync(loginRequest.Username);
                if (existingUser == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "The user was not found."));
                }

                var verificationResult = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, loginRequest.Password);
                if (verificationResult != PasswordVerificationResult.Success)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid password."));
                }

                TokensDTO tokensDTO = await _tokenService.GenerateTokensAsync(existingUser.Username);

                return new LoginResponse
                {
                    AccessToken = tokensDTO.AccessToken,
                    RefreshToken = tokensDTO.RefreshToken,
                    Id = existingUser.Id.ToString(),
                };
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

        public async Task ValidateTokenAsync(ValidateTokenRequest validateTokenRequest)
        {
            try
            {
                var claimsPrincipal = _tokenService.ValidateToken(validateTokenRequest.AccessToken);

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

                if (user.AccessToken != validateTokenRequest.AccessToken)
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

        public async Task<ExtendTokenResponse> ExtendTokenAsync(ExtendTokenRequest extendTokenRequest)
        {
            try
            {
                var claimsPrincipal = _tokenService.ValidateToken(extendTokenRequest.AccessToken);

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

                if (user.RefreshToken != extendTokenRequest.RefreshToken)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, $"Wrong refresh token."));
                }

                if (user.AccessToken != extendTokenRequest.AccessToken)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, $"Wrong access token."));
                }

                return new ExtendTokenResponse
                {
                    AccessToken = await _tokenService.GenerateAccessTokenAsync(user.Id),
                };
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