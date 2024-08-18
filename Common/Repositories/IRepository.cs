using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Common.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        IQueryable<T> GetWhere(Expression<Func<T, bool>> method,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool tracking = true);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetAsync(int id);
        IQueryable<T> GetAll(bool noTracking = false);
        T Get(int id);
        bool Delete(int id);
        bool Delete(T entity);
        bool DeleteRange(ICollection<T> entities);
        void Update(T entity);
        T Add(T entity);
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);
    }
}
