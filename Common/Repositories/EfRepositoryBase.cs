using Microsoft.EntityFrameworkCore;

namespace Common.Repositories;

 public class EfRepositoryBase<T> : IRepository<T>
         where T : BaseEntity
 {
     public readonly DbContext Context;
     private readonly DbSet<T> _entitySet;

     public EfRepositoryBase(DbContext context)
     {
         Context = context;
         _entitySet = Context.Set<T>();
     }
     public virtual T Get(int id)
     {
         return _entitySet.Find(id);
     }
     public virtual async Task<T> GetAsync(int id, CancellationToken cancellationToken)
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
         return objToReturn.Entity;
     }
     public virtual async Task<T> AddAsync(T entity)
     {
         return (await _entitySet.AddAsync(entity)).Entity;
     }
     public virtual void AddRange(List<T> entities)
     {
         var enumerable = entities as IList<T> ?? entities.ToList();
         _entitySet.AddRange(enumerable);
     }
     public virtual async Task AddRangeAsync(ICollection<T> entities)
     {
         if (entities == null || !entities.Any())
         {
             return;
         }
         await _entitySet.AddRangeAsync(entities);
     }

     public virtual void Update(T entity)
     {
         Context.Entry(entity).State = EntityState.Modified;
     }


     public virtual bool Delete(int id)
     {
         var obj = _entitySet.Find(id);
         if (obj == null)
         {
             return false;
         }
         _entitySet.Remove(obj);
         return true;
     }

     public virtual bool Delete(T entity)
     {
         if (entity == null)
         {
             return false;
         }
         Context.Entry(entity).State = EntityState.Deleted;
         return true;
     }

     public virtual bool DeleteRange(ICollection<T> entities)
     {
         if (entities == null || !entities.Any())
         {
             return false;
         }
         _entitySet.RemoveRange(entities);
         return true;
     }
     public virtual void SaveChanges()
     {
         Context.SaveChanges();
     }

     public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
     {
         await Context.SaveChangesAsync(cancellationToken);
     }
 }