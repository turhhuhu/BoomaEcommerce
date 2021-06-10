using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Data.InMemory
{
    public class InMemoryUserRepository : IRepository<User>
    {
        public async Task<IEnumerable<User>> FindAllAsync()
        {
            return InMemoryUserStore.Users.Values.ToList();
        }

        public async Task<IEnumerable<User>> FilterByAsync(Expression<Func<User, bool>> predicateExp)
        {
            return InMemoryUserStore.Users.Values.Where(predicateExp.Compile());
        }

        public async Task<IEnumerable<TMapped>> FilterByAsync<TMapped>(Expression<Func<User, bool>> predicateExp, Expression<Func<User, TMapped>> mapExp)
        {
            return InMemoryUserStore.Users.Values.Where(predicateExp.Compile()).Select(mapExp.Compile());
        }

        public async Task<User> FindOneAsync(Expression<Func<User, bool>> predicateExp)
        {
            return InMemoryUserStore.Users.Values.Where(predicateExp.Compile()).FirstOrDefault();
        }

        public Task<User> FindByIdAsync(Guid guid)
        {
            return Task.FromResult(InMemoryUserStore.Users.Values.FirstOrDefault(x => x.Guid == guid));
        }

        public Task InsertOneAsync(User entity)
        {
            InMemoryUserStore.Users[entity.Guid.ToString()] = entity;
            return Task.CompletedTask;
        }

        public Task InsertManyAsync(IEnumerable<User> entities)
        {
            foreach (var entity in entities)
            {
                InMemoryUserStore.Users[entity.Guid.ToString()] = entity;
            }
            return Task.CompletedTask;
        }

        public Task ReplaceOneAsync(User entity)
        {
            InMemoryUserStore.Users[entity.Guid.ToString()] = entity;
            return Task.CompletedTask;
        }

        public Task DeleteOneAsync(Expression<Func<User, bool>> predicate)
        {
            var usr = InMemoryUserStore.Users.Values.FirstOrDefault(predicate.Compile());
            if (usr != null)
            {
                InMemoryUserStore.Users.TryRemove(usr.Guid.ToString(), out _);
            }
            return Task.CompletedTask;
        }

        public Task DeleteByIdAsync(Guid guid)
        {
            InMemoryUserStore.Users.TryRemove(guid.ToString(), out _);
            return Task.CompletedTask;
        }

        public Task DeleteManyAsync(Expression<Func<User, bool>> predicate)
        {
            var p = predicate.Compile();
            foreach (var user in InMemoryUserStore.Users.Values)
            {
                if (p(user))
                {
                    DeleteByIdAsync(user.Guid);
                }
            }

            return Task.CompletedTask;
        }

        public void DeleteRange(IEnumerable<User> entities)
        {
            foreach (var user in entities)
            {
                DeleteByIdAsync(user.Guid);
            }
        }

        public Task<TType> FindByIdAsync<TType>(Guid guid) where TType : class, IBaseEntity
        {
            throw new NotImplementedException();
        }

        public void Attach(User entity)
        {
        }
    }
}
