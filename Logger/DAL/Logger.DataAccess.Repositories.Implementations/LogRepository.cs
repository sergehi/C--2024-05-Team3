using Microsoft.EntityFrameworkCore;

namespace Logger.DataAccess.Repositories.Implementations
{
    public class LogRepository : Common.Repositories.Repository<Entities.Log, long>
    {
        public LogRepository(DbContext context) : base(context) { }
    }
}
