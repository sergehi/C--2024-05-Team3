using Logger.BusinessLogic.DTO.Log;

namespace Logger.BusinessLogic.Services.Abstractions
{
    public interface ILogService
    {
        Task<IEnumerable<LogDTO>> GetLogsAsync(FilterLogDTO filterLogDTO);
        Task<IEnumerable<LogDTO>> GetPagedLogsAsync(FilterLogDTO filterLogDTO);
        Task<long> CreateAsync(CreateLogDTO createLogDTO);
        Task DeleteAsync(DateTime begin, DateTime end );
    }
}
