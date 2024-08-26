using Logger.BusinessLogic.DTO.Log;

namespace Logger.BusinessLogic.Services.Abstractions
{
    public interface ILogServise
    {
        Task<long> CreateAsync(CreateLogDTO createLogDTO);
        void DeleteRange(IEnumerable<LogDTO> collectionLogDTO);
    }
}
