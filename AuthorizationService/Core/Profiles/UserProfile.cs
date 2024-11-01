using AutoMapper;
using AuthorizationService.Core.Entities;
using AuthorizationService.Shared.DTOs;
using AuthorizationService.Shared.Protos;

namespace AuthorizationService.Core.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterDTO, User>();
            CreateMap<RegisterRequest, RegisterDTO>();
        }
    }
}