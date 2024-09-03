using AutoMapper;
using Logger.BusinessLogic.DTO.Log;
using Logger.DataAccess.Entities;

namespace Logger.BusinessLogic.Services.Implementations.Mapping
{
    /// <summary>
    /// Профиль автомаппера для сущности Лог.
    /// </summary>
    public class LogMappingsProfile : Profile
    {
        public LogMappingsProfile()
        {
            CreateMap<Log, LogDTO>()
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => new DateTime(src.Time)));
            CreateMap<LogDTO, Log>()
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time.Ticks));
            CreateMap<CreateLogDTO, Log>()
                .ForMember(m => m.Id, map => map.Ignore())
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time.Ticks));
        }
    }
}
