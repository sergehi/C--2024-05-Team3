using Logger.DataAccess.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Logger.DataAccess.Repositories.Implementations
{
    public class LogRepository : Common.Repositories.Repository<Entities.Log, long>, ILogRepository
    {
        public LogRepository(DbContext context) : base(context) { }
    }
}
