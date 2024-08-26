using AutoMapper;
using Logger.BusinessLogic.DTO.Log;
using Logger.DataAccess.Entities;
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
            //CreateMap<CreateLogDTO, Log>()
            //    .ForMember(m => m.Id, map => map.Ignore());
        }
    }
}
