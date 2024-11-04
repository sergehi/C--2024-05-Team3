using AutoMapper;
using AuthorizationService.Core.Entities;
using AuthorizationService.Shared.DTOs;
using AuthorizationService.Shared.Protos;

namespace AuthorizationService.Core.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterDTO, User>();
            CreateMap<RegisterRequest, RegisterDTO>();
            CreateMap<LoginRequest, LoginDTO>();
            CreateMap<ValidateTokenRequest, ValidateTokenDTO>();
            CreateMap<TokensDTO, LoginResponse>();
            CreateMap<ExtendTokenRequest, TokensDTO>();
        }
    }
}