using AutoMapper;
using Logger.BusinessLogic.DTO.Log;
using LoggerService.Models.Log;

namespace LoggerService.Mapping
{
    /// <summary>
    /// Профиль автомаппера для сущности Лог.
    /// </summary>
    public class LogMappingsProfile : Profile
    {
        public LogMappingsProfile()
        {
            CreateMap<LogDTO, LogModel>();
            CreateMap<LogModel, LogDTO>();
            CreateMap<FilterLogModel, FilterLogDTO>()
                .ForMember(dest => dest.BeginTime, opt => opt.MapFrom(src => new DateTime(src.BeginTime)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => new DateTime(src.EndTime)))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => StringToGuid(src.UserId)))
                .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => StringToGuid(src.EntityType)))
                .ForMember(dest => dest.EntityPK, opt => opt.MapFrom(src => StringToGuid(src.EntityPk)));
            CreateMap<LogDTO, LogItem>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time.Ticks))
                .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => src.EntityType.ToString()))
                .ForMember(dest => dest.EntityPk, opt => opt.MapFrom(src => src.EntityPK.ToString()));
            CreateMap<CreatingLogModel, CreateLogDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => StringToGuid(src.UserId)))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => new DateTime(src.Time)))
                .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => StringToGuid(src.EntityType)))
                .ForMember(dest => dest.EntityPK, opt => opt.MapFrom(src => StringToGuid(src.EntityPk)));
        }

        private static Guid StringToGuid(string value)
        {
            if (!Guid.TryParse(value, out Guid guid))
                guid = Guid.Empty;
            return guid;
        }
    }
}
