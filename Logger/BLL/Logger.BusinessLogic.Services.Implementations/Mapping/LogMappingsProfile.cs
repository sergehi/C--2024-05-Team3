﻿using AutoMapper;
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
            CreateMap<Log, LogDTO>();
            CreateMap<LogDTO, Log>();
            CreateMap<CreateLogDTO, Log>()
                .ForMember(m => m.Id, map => map.Ignore());
        }
    }
}
