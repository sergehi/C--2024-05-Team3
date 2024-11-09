using AutoMapper;
using AuthorizationService.Core.Entities;
using AuthorizationService.Shared.Protos;

namespace AuthorizationService.Core.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterRequest, User>();
        }
    }
}