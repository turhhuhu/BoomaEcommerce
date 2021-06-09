using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using Microsoft.EntityFrameworkCore;
using BoomaEcommerce.Data.EfCore;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    public class EfCoreRepository<T, TDbContext> : IRepository<T> 
        where T : class, IBaseEntity
        where TDbContext : DbContext
    {
        protected readonly TDbContext DbContext;

        public EfCoreRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }
        public virtual async Task<IEnumerable<T>> FindAllAsync()
        {
            return await DbContext.Set<T>().Include(DbContext.GetIncludePaths(typeof(T))).OrderByDescending(x => x.Guid).AsSplitQuery().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FilterByAsync(Expression<Func<T, bool>> predicateExp)
        {
            return await DbContext.Set<T>().Include(DbContext.GetIncludePaths(typeof(T))).OrderByDescending(x => x.Guid).AsSplitQuery().Where(predicateExp).ToListAsync();
        }

        public virtual async Task<IEnumerable<TMapped>> FilterByAsync<TMapped>(Expression<Func<T, bool>> predicateExp, Expression<Func<T, TMapped>> mapExp)
        {
            return await DbContext.Set<T>().Include(DbContext.GetIncludePaths(typeof(T))).OrderByDescending(x => x.Guid).AsSplitQuery().Where(predicateExp).Select(mapExp).ToListAsync();
        }

        public virtual Task<T> FindOneAsync(Expression<Func<T, bool>> predicateExp)
        {
            return DbContext.Set<T>().Include(DbContext.GetIncludePaths(typeof(T))).OrderByDescending(x => x.Guid).AsSplitQuery().FirstOrDefaultAsync(predicateExp);
        }

        public virtual async Task<T> FindByIdAsync(Guid guid)
        {
            return await FindOneAsync((x) => x.Guid == guid);
        }

        public virtual async Task InsertOneAsync(T entity)
        {
            await DbContext.AddAsync(entity);
        }

        public virtual async Task InsertManyAsync(IEnumerable<T> entities)
        {
            await DbContext.AddRangeAsync(entities);
        }

        public virtual Task ReplaceOneAsync(T entity)
        {
            DbContext.Update(entity);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteOneAsync(Expression<Func<T, bool>> predicate)
        {
            var dbSet = DbContext.Set<T>();
            var toRemove = await dbSet.FirstOrDefaultAsync(predicate);
            if (toRemove != null)
            {
                dbSet.Remove(toRemove);
            }
        }

        public virtual async Task DeleteByIdAsync(Guid guid)
        {
            var dbSet = DbContext.Set<T>();
            var toRemove = await dbSet.FindAsync(guid);
            if (toRemove != null)
            {
                dbSet.Remove(toRemove);
            }
        }

        public virtual async Task DeleteManyAsync(Expression<Func<T, bool>> predicate)
        {
            var dbSet = DbContext.Set<T>();
            var toRemove = await dbSet.Where(predicate).ToListAsync();

            if (toRemove.Count > 0)
            {
                dbSet.RemoveRange(toRemove);
            }
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            DbContext.Set<T>().RemoveRange(entities);
        }

        public virtual Task<TType> FindByIdAsync<TType>(Guid guid) where TType : class, IBaseEntity
        {
            return DbContext.Set<T>().Include(DbContext.GetIncludePaths(typeof(T))).AsSplitQuery().OrderByDescending(x => x.Guid).OfType<TType>().FirstOrDefaultAsync(e => e.Guid == guid);
        }

        public virtual void Attach(T entity)
        { 
            DbContext.Set<T>().Attach(entity);
        }
    }
}

