using AutoMapper;
using Logger.BusinessLogic.Services.Abstractions;
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
    }
}
