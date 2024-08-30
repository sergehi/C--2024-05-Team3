using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Common.Repositories;

public abstract class Repository<T, TPrimaryKey>
    where T : class, IEntity<TPrimaryKey>
{
    private readonly DbSet<T> _entitySet;
    public readonly DbContext Context;

    public Repository(DbContext context)
    {
        Context = context;
        _entitySet = Context.Set<T>();
    }

    public virtual T? Get(TPrimaryKey id)
    {
        return _entitySet.Find(id);
    }
    public virtual async Task<T?> GetAsync(TPrimaryKey id)
    {
        return await _entitySet.FindAsync(id);
    }
    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().FirstOrDefaultAsync(predicate);
    }
    public virtual IQueryable<T> GetAll(bool noTracking = false)
    {
        return noTracking ? _entitySet.AsNoTracking() : _entitySet;
    }
    public virtual async Task<IQueryable<T>> GetAllAsync(CancellationToken cancellationToken, bool noTracking = false)
    {
        return (await GetAll(noTracking).ToListAsync(cancellationToken)).AsQueryable();
    }
    public virtual IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, 
        object>>? includes = null, bool noTracking = false)
    {
        var query = Context.Set<T>().Where(predicate);
        if (includes != null)
            query = includes(query);
        return noTracking ? query.AsNoTracking() : query;
    }
    public virtual async Task<IQueryable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, 
        IIncludableQueryable<T, object>>? includes = null, bool noTracking = false)
    {
        return (await GetWhere(predicate, includes, noTracking).ToArrayAsync()).AsQueryable();
    }

    public virtual T Add(T entity)
    {
        return _entitySet.Add(entity).Entity;
    }
    public virtual async Task<T> AddAsync(T entity)
    {
        return (await _entitySet.AddAsync(entity)).Entity;
    }
    public virtual void AddRange(IEnumerable<T> entities)
    {
        _entitySet.AddRange(entities);
    }
    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _entitySet.AddRangeAsync(entities);
    }


    public virtual void Update(T entity)
    {
        _entitySet.Update(entity);
    }
    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        _entitySet.UpdateRange(entities);
    }


    public virtual void Delete(T entity)
    {
        _entitySet.Remove(entity);
    }
    public virtual void Delete(TPrimaryKey id)
    {
        var obj = _entitySet.Find(id);
        if (obj == null)
            return;
        _entitySet.Remove(obj);
    }
    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        _entitySet.RemoveRange(entities);
    }


    public virtual void SaveChanges()
    {
        using var transaction = Context.Database.BeginTransaction();
        try
        {
            Context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        using var transaction = Context.Database.BeginTransaction();
        try
        {
            await Context.SaveChangesAsync(cancellationToken);
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}