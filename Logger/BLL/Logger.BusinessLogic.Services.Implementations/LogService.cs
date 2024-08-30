using AutoMapper;
using Logger.BusinessLogic.DTO.Log;
using Logger.BusinessLogic.Services.Abstractions;
using Logger.DataAccess.Entities;
using Logger.DataAccess.Repositories.Abstractions;

namespace Logger.BusinessLogic.Services.Implementations
{
    public class LogService : ILogService
    {
        private readonly IMapper _mapper;
        private readonly ILogRepository _logRepository;

        public LogService(IMapper mapper, ILogRepository logRepository)
        {
            _mapper = mapper;
            _logRepository = logRepository;
        }

        public async Task<IEnumerable<LogDTO>> GetLogsAsync(FilterLogDTO filter)
        {
            return _mapper.Map<IEnumerable<LogDTO>>(await _logRepository.GetLogsAsync(filter.BeginTime, filter.EndTime, filter.Action, filter.UserId, filter.EntityType, filter.EntityPK));
        }
        public async Task<IEnumerable<LogDTO>> GetPagedLogsAsync(FilterLogDTO filter)
        {
            return _mapper.Map<IEnumerable<LogDTO>>(await _logRepository.GetPagedLogsAsync(filter.BeginTime, filter.EndTime, filter.Action, filter.UserId, filter.EntityType, filter.EntityPK, filter.Page, filter.ItemsPerPage));
        }

        public async Task<long> CreateAsync(CreateLogDTO createLogDTO)
        {
            Log log = _mapper.Map<CreateLogDTO, Log>(createLogDTO);
            log = await _logRepository.AddAsync(log);
            await _logRepository.SaveChangesAsync();
            return log.Id;
        }
        public async Task DeleteAsync(DateTime begin, DateTime end)
        {
            await _logRepository.DeleteLogsAsync(begin, end);
            await _logRepository.SaveChangesAsync();
        }
    }
}
