namespace Common.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        IQueryable<T> GetAll(bool noTracking = false);
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false);
        T Get(int id);
        Task<T> GetAsync(int id, CancellationToken cancellationToken);
        bool Delete(int id);
        bool Delete(T entity);
        bool DeleteRange(ICollection<T> entities);
        void Update(T entity);
        T Add(T entity);
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        void SaveChanges();
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
