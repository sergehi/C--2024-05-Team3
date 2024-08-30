using Logger.DataAccess.Entities;

namespace Logger.DataAccess.Repositories.Abstractions
{
    public interface ILogRepository : Common.Repositories.IRepository<Log, long>
    {
        Task<IEnumerable<Log>> GetLogsAsync(DateTime begin_time, DateTime end_time, ELogAction action, Guid user_id, Guid entity_type, Guid entity_pk);
        Task<IEnumerable<Log>> GetPagedLogsAsync(DateTime begin_time, DateTime end_time, ELogAction action, Guid user_id, Guid entity_type, Guid entity_pk, int page, int items_per_page);
        Task DeleteLogsAsync(DateTime begin_time, DateTime end_time);
    }
}
