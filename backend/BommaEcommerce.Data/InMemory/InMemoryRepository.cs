using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Data.InMemory
{
    public static class RepoContainer
    {
        public static Dictionary<Type, Dictionary<Guid, BaseEntity>> AllEntities { get; set; } = new();
    }
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        public InMemoryRepository()
        {
            var dict = new Dictionary<Guid, T>();
            RepoContainer.AllEntities.TryAdd(typeof(T), dict.ToDictionary(x => x.Key, x => (BaseEntity)x.Value));
        }

        public Task<IEnumerable<T>> FindAllAsync()
        {
            var entities = RepoContainer.AllEntities[typeof(T)];
            return Task.FromResult<IEnumerable<T>>(entities.Values.Select(x => (T)x));
        }

        public Task<IEnumerable<T>> FilterByAsync(Expression<Func<T, bool>> predicateExp)
        {
            var entities = RepoContainer.AllEntities[typeof(T)];
            var predicate = predicateExp.Compile();
            return Task.FromResult(entities.Values.Select(x => (T)x).Where(x => predicate(x)));
        }

        public Task<IEnumerable<TMapped>> FilterByAsync<TMapped>(Expression<Func<T, bool>> predicateExp, Expression<Func<T, TMapped>> mapExp)
        {
            var entities = RepoContainer.AllEntities[typeof(T)];
            var predicate = predicateExp.Compile();
            var mapFunc = mapExp.Compile();
            return Task.FromResult(entities.Values.Select(x => (T)x).Where(predicate).Select(mapFunc));
        }

        public async Task<T> FindOneAsync(Expression<Func<T, bool>> predicateExp)
        {
            var result = await FilterByAsync(predicateExp);
            return result.FirstOrDefault();
        }

        public Task<T> FindByIdAsync(Guid guid)
        {
            var entities = RepoContainer.AllEntities[typeof(T)];
            return entities.TryGetValue(guid, out var entity) 
                ? Task.FromResult((T)entity) 
                : Task.FromResult<T>(null);
        }

        public virtual Task InsertOneAsync(T entity)
        {
            var entities = RepoContainer.AllEntities[typeof(T)];
            entities.TryAdd(entity.Guid, entity);
            return Task.CompletedTask;
        }

        public async Task InsertManyAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                await InsertOneAsync(entity);
            }
        }

        public Task ReplaceOneAsync(T entity)
        {
            var entities = RepoContainer.AllEntities[typeof(T)];
            if (entities.ContainsKey(entity.Guid))
            {
                entities[entity.Guid] = entity;
            }
            return Task.CompletedTask;
        }

        public virtual Task DeleteOneAsync(Expression<Func<T, bool>> predicate)
        {
            var entities = RepoContainer.AllEntities[typeof(T)];
            var pred = predicate.Compile();
            foreach (var (guid, entity) in entities)
            {
                if (pred((T)entity))
                {
                    entities.Remove(guid, out _);
                    return Task.CompletedTask;
                }
            }
            return Task.CompletedTask;
        }

        public virtual Task DeleteByIdAsync(Guid guid)
        {
            var entity = RepoContainer.AllEntities[typeof(T)];
            entity.Remove(guid, out _);
            return Task.CompletedTask;
        }

        public Task DeleteManyAsync(Expression<Func<T, bool>> predicate)
        {
            var entities = RepoContainer.AllEntities[typeof(T)];
            var pred = predicate.Compile();
            var keysToRemove = entities.Keys.Where(guid => pred((T)entities[guid]));

            foreach (var key in keysToRemove)
            {
                entities.Remove(key, out _);
            }
            return Task.CompletedTask;
        }

        public Task<TType> FindByIdAsync<TType>(Guid guid) 
            where TType : BaseEntity
        {
            var entities = RepoContainer.AllEntities[typeof(T)];
            return entities.TryGetValue(guid, out var entity)
                ? Task.FromResult((TType)entity)
                : Task.FromResult<TType>(null);
        }

        public void Attach(T entity)
        {
            
        }
    }
}
