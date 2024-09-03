using Logger.DataAccess.Entities;
using Logger.DataAccess.EntityFramework;
using Logger.DataAccess.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Logger.DataAccess.Repositories.Implementations
{
    public class LogRepository : Common.Repositories.Repository<Log, long>, ILogRepository
    {
        public LogRepository(LoggerContext context) : base(context) { }

        /// <summary>
        /// Получить выборку логов.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Log>> GetLogsAsync(DateTime begin_time, DateTime end_time, ELogAction action, Guid user_id, Guid entity_type, Guid entity_pk)
        {
            NpgsqlParameter[] parameters =
            [
                new NpgsqlParameter("@time_b", begin_time.Ticks),
                new NpgsqlParameter("@time_e", end_time.Ticks),
                new NpgsqlParameter("@e_action", (int)action),
                user_id == Guid.Empty ? new NpgsqlParameter("@user_id", string.Empty) : new NpgsqlParameter("@user_id", user_id.ToString()),
                entity_type == Guid.Empty ? new NpgsqlParameter("@entity_type", string.Empty) : new NpgsqlParameter("@entity_type", entity_type.ToString()),
                entity_pk == Guid.Empty ? new NpgsqlParameter("@entity_pk", string.Empty) : new NpgsqlParameter("@entity_pk", entity_pk.ToString())
            ];
            var query = Context.Set<Log>().FromSqlRaw("SELECT * FROM public.loglist(@time_b, @time_e, @e_action, @user_id, @entity_type, @entity_pk)", parameters);
            return await query.ToListAsync();
        }

        /// <summary>
        /// Получить постраничную выборку логов.
        /// </summary>
        /// <param name="page"> Номер страницы. </param>
        /// <param name="items_per_page"> Количество элементов на странице. </param>
        /// <returns></returns>
        public async Task<IEnumerable<Log>> GetPagedLogsAsync(DateTime begin_time, DateTime end_time, ELogAction action, Guid user_id, Guid entity_type, Guid entity_pk, int page, int items_per_page)
        {
            NpgsqlParameter[] parameters =
            [
                new NpgsqlParameter("@time_b", begin_time.Ticks),
                new NpgsqlParameter("@time_e", end_time.Ticks),
                new NpgsqlParameter("@e_action", (int)action),
                user_id == Guid.Empty ? new NpgsqlParameter("@user_id", string.Empty) : new NpgsqlParameter("@user_id", user_id.ToString()),
                entity_type == Guid.Empty ? new NpgsqlParameter("@entity_type", string.Empty) : new NpgsqlParameter("@entity_type", entity_type.ToString()),
                entity_pk == Guid.Empty ? new NpgsqlParameter("@entity_pk", string.Empty) : new NpgsqlParameter("@entity_pk", entity_pk.ToString())
            ];
            var query = Context.Set<Log>().FromSqlRaw("SELECT * FROM public.loglist(@time_b, @time_e, @e_action, @user_id, @entity_type, @entity_pk)", parameters);
            return await query
                .Skip((page - 1) * items_per_page)
                .Take(items_per_page)
                .ToListAsync();
        }

        /// <summary>
        /// Удалить логи за период.
        /// </summary>
        /// <param name="begin_time"> Начало периода. </param>
        /// <param name="end_time"> Конец периода. </param>
        /// <returns></returns>
        public async Task DeleteLogsAsync(DateTime begin_time, DateTime end_time)
        {
            Task task = new Task(() =>
            {
                var query = Context.Set<Log>().Where(e => e.Time >= begin_time.Ticks && e.Time <= end_time.Ticks);
                DeleteRange(query);
            });
            task.Start();
            await task;
        }
    }
}
