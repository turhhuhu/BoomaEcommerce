using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Data
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        public ConcurrentDictionary<Guid, T> Entities { get; set; } = new();

        public Task<IEnumerable<T>> FindAllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(Entities.Values);
        }

        public Task<IEnumerable<T>> FilterByAsync(Expression<Func<T, bool>> predicateExp)
        {
            var predicate = predicateExp.Compile();
            return Task.FromResult(Entities.Values.Where(x => predicate(x)));
        }

        public async Task<IEnumerable<TMapped>> FilterByAsync<TMapped>(Expression<Func<T, bool>> predicateExp, Expression<Func<T, TMapped>> mapExp)
        {
            var result = await FilterByAsync(predicateExp);
            var mapFunc = mapExp.Compile();
            return result.Select(mapFunc);
        }

        public async Task<T> FindOneAsync(Expression<Func<T, bool>> predicateExp)
        {
            var result = await FilterByAsync(predicateExp);
            return result.FirstOrDefault();
        }

        public Task<T> FindByIdAsync(Guid guid)
        {
            return Entities.TryGetValue(guid, out var entity) 
                ? Task.FromResult(entity) 
                : null;
        }

        public Task InsertOneAsync(T entity)
        {
            Entities.TryAdd(entity.Guid, entity);
            return Task.CompletedTask;
        }

        public async Task InsertManyAsync(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                await InsertOneAsync(entity);
            }
        }

        public Task ReplaceOneAsync(T entity)
        {
            if (Entities.ContainsKey(entity.Guid))
            {
                Entities[entity.Guid] = entity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteOneAsync(Expression<Func<T, bool>> predicate)
        {
            var pred = predicate.Compile();
            foreach (var (guid, entity) in Entities)
            {
                if (pred(entity))
                {
                    Entities.Remove(guid, out _);
                    return Task.CompletedTask;
                }
            }
            return Task.CompletedTask;
        }

        public Task DeleteByIdAsync(Guid guid)
        {
            Entities.Remove(guid, out _);
            return Task.CompletedTask;
        }

        public Task DeleteManyAsync(Expression<Func<T, bool>> predicate)
        {
            var pred = predicate.Compile();
            var keysToRemove = Entities.Keys.Where(guid => pred(Entities[guid]));

            foreach (var key in keysToRemove)
            {
                Entities.Remove(key, out _);
            }
            return Task.CompletedTask;
        }
    }
}
