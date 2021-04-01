using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Data
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Filters T entities by a filter predicate expression.
        /// </summary>
        /// <param name="predicateExp"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the T entity collection that satisfied the predicate.
        /// </returns>
        Task<IEnumerable<T>> FilterByAsync(
            Expression<Func<T, bool>> predicateExp);

        /// <summary>
        /// Filters T entities by a filter predicate expression
        /// and applies the mapping expression on it.
        /// </summary>
        /// <typeparam name="TMapped"></typeparam>
        /// <param name="predicateExp"></param>
        /// <param name="mapExp"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the TMapped entity collection that satisfied the predicate
        /// and the mapping was applied upon.
        /// </returns>
        Task<IEnumerable<TMapped>> FilterByAsync<TMapped>(
            Expression<Func<T, bool>> predicateExp,
            Expression<Func<T, TMapped>> mapExp);

        /// <summary>
        /// Finds one T entity that satisfies the predicate.
        /// </summary>
        /// <param name="predicateExp"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The T entity that satisfied the predicate.
        /// </returns>
        Task<T> FindOneAsync(Expression<Func<T, bool>> predicateExp);

        /// <summary>
        /// Finds one T entity with the provided guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The T entity with the guid if found, otherwise null.
        /// </returns>
        Task<T> FindByIdAsync(Guid guid);

        /// <summary>
        /// Inserts one T entity entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsertOneAsync(T entity);

        /// <summary>
        /// Inserts a collection of T entities.
        /// </summary>
        /// <param name="entities"></param>
        Task InsertManyAsync(ICollection<T> entities);

        /// <summary>
        /// Replaces existing T entity with new T entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task ReplaceOneAsync(T entity);

        /// <summary>
        /// Deletes an entity that satisfies the provided predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task DeleteOneAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Deletes an entity that has the provided guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task DeleteByIdAsync(Guid guid);

        /// <summary>
        /// Deletes all entities that satisfy the predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task DeleteManyAsync(Expression<Func<T, bool>> predicate);
    }
}
