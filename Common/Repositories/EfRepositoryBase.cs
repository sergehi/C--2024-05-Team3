using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Common.Repositories;

public class Repository<T> : IRepository<T>
    where T : BaseEntity
{
    public readonly DbContext Context;
    private readonly DbSet<T> _entitySet;

    public Repository(DbContext context)
    {
        Context = context;
        _entitySet = Context.Set<T>();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public IQueryable<T> GetWhere(Expression<Func<T, bool>> method,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null,
        bool tracking = true)
    {
        var query = Context.Set<T>().Where(method);
        if (includes != null)
            query = includes(query);
        if (!tracking)
            query = query.AsNoTracking();
        return query;
    }

    public virtual T Get(int id)
    {
        return _entitySet.Find(id);
    }

    public virtual async Task<T> GetAsync(int id)
    {
        return await _entitySet.FindAsync((object)id);
    }

    public virtual IQueryable<T> GetAll(bool asNoTracking = false)
    {
        return asNoTracking ? _entitySet.AsNoTracking() : _entitySet;
    }

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false)
    {
        return await GetAll().ToListAsync(cancellationToken);
    }

    public virtual T Add(T entity)
    {
        var objToReturn = _entitySet.Add(entity);
        Context.SaveChanges();
        return objToReturn.Entity;
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        Context.Entry(entity).State = EntityState.Added;
        await Context.SaveChangesAsync();
        return entity;
    }

    public virtual void AddRange(List<T> entities)
    {
        var enumerable = entities as IList<T> ?? entities.ToList();
        _entitySet.AddRange(enumerable);
        Context.SaveChanges();
    }

    public virtual async Task AddRangeAsync(ICollection<T> entities)
    {
        if (entities == null || !entities.Any())
        {
            return;
        }

        _entitySet.AddRangeAsync(entities);
        await Context.SaveChangesAsync();
    }

    public virtual void Update(T entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
        Context.SaveChanges();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync();
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        await Context.SaveChangesAsync();
    }

    public virtual bool Delete(int id)
    {
        var obj = _entitySet.Find(id);
        if (obj == null)
        {
            return false;
        }

        _entitySet.Remove(obj);
        Context.SaveChanges();
        return true;
    }

    public virtual bool Delete(T entity)
    {
        if (entity == null)
        {
            return false;
        }

        Context.Entry(entity).State = EntityState.Deleted;
        Context.SaveChanges();
        return true;
    }

    public virtual bool DeleteRange(ICollection<T> entities)
    {
        if (entities == null || !entities.Any())
        {
            return false;
        }

        _entitySet.RemoveRange(entities);
        Context.SaveChanges();
        return true;
    }
}