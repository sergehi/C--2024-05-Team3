using AutoMapper;
using Logger.BusinessLogic.DTO.Log;
using Logger.BusinessLogic.Services.Abstractions;
using Logger.DataAccess.Entities;
using Logger.DataAccess.Repositories.Abstractions;

namespace Logger.BusinessLogic.Services.Implementations
{
    public class LogService : ILogServise
    {
        private readonly IMapper _mapper;
        private readonly ILogRepository _logRepository;

        public LogService(IMapper mapper, ILogRepository logRepository)
        {
            _mapper = mapper;
            _logRepository = logRepository;
        }

        public async Task<long> CreateAsync(CreateLogDTO createLogDTO)
        {
            Log log = _mapper.Map<CreateLogDTO, Log>(createLogDTO);
            log = await _logRepository.AddAsync(log);
            await _logRepository.SaveChangesAsync();
            return log.Id;
        }
        public async void DeleteRange(IEnumerable<LogDTO> collectionLogDTO)
        {
            IEnumerable<Log> logs = _mapper.Map<IEnumerable<LogDTO>, IEnumerable<Log>>(collectionLogDTO);
            _logRepository.DeleteRange(logs);
            await _logRepository.SaveChangesAsync();
        }
    }
}
