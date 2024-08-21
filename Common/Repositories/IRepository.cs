using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Common.Repositories
{
    public interface IRepository<T, TPrimaryKey>
        where T : IEntity<TPrimaryKey>
    {
        T? Get(TPrimaryKey id);
        Task<T?> GetAsync(TPrimaryKey id);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll(bool noTracking = false);
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken, bool noTracking = false);
        IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool noTracking = false);


        T Add(T entity);
        Task<T> AddAsync(T entity);
        void AddRange(IEnumerable<T> entities);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);


        void Delete(T entity);
        void Delete(TPrimaryKey id);
        void DeleteRange(IEnumerable<T> entities);


        void SaveChanges();
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
