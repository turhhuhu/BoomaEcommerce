using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Expression = Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression;

namespace BommaEcommerce.Services.Tests
{
    public static class DalMockFactory
    {
        public static Mock<UserManager<User>> MockUserManager(User user = null, List<User> ls = null)
        {
            if (user == null & ls == null)
            {
                throw new NullReferenceException("User or user list must be provided to mock the UserManager.");
            }
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            mgr.Object.UserValidators.Add(new UserValidator<User>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<User>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            mgr.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<User, string>((x, y) => ls?.Add(x));

            mgr.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            mgr.Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var foundUser = user ?? ls.First();

            mgr.Setup(userManager => userManager.FindByNameAsync(foundUser.UserName))
                .ReturnsAsync(user);

            return mgr;
        }

        public static Mock<IRepository<TEntity>> MockRepository<TEntity>(IDictionary<Guid, TEntity> entities)
            where TEntity : BaseEntity
        {
            var repoMock = new Mock<IRepository<TEntity>>();
            repoMock.Setup(x => x.DeleteByIdAsync(It.IsAny<Guid>()))
                .Callback<Guid>(guid => entities.Remove(guid, out _));

            repoMock.Setup(x => x.DeleteManyAsync(It.IsAny<Expression<Func<TEntity, bool>>>()))
                .Callback<Expression<Func<TEntity, bool>>>(predicate =>
                {
                    var pred = predicate.Compile();
                    var keysToRemove = entities.Keys.Where(guid => pred(entities[guid]));

                    foreach (var key in keysToRemove)
                    {
                        entities.Remove(key, out _);
                    }
                });

            repoMock.Setup(x => x.DeleteOneAsync(It.IsAny<Expression<Func<TEntity, bool>>>()))
                .Callback<Expression<Func<TEntity, bool>>>(predicate =>
                {
                    var pred = predicate.Compile();
                    var keyToRemove = entities.Keys.FirstOrDefault(guid => pred(entities[guid]));

                    if (keyToRemove != default)
                        entities.Remove(keyToRemove, out _);
                });

            repoMock.Setup(x => x.FilterByAsync(It.IsAny<Expression<Func<TEntity, bool>>>()))
                .ReturnsAsync((Expression<Func<TEntity, bool>> x) => 
                    entities.Values.Where(entity => x.Compile()(entity)));

            repoMock.Setup(x => x.InsertOneAsync(It.IsAny<TEntity>()))
                .Callback<TEntity>(entity => entities.Add(entity.Guid, entity));

            repoMock.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<TEntity, bool>>>()))
                .ReturnsAsync((Expression<Func<TEntity, bool>> x) =>
                    entities.Values.FirstOrDefault(entity => x.Compile()(entity)));

            repoMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) => entities[guid]);

            repoMock.Setup(x => x.InsertManyAsync(It.IsAny<ICollection<TEntity>>()))
                .Callback<ICollection<TEntity>>(entitiesToAdd =>
                    entitiesToAdd.ToList().ForEach(ent => entities.Add(ent.Guid, ent)));

            repoMock.Setup(x => x.ReplaceOneAsync(It.IsAny<TEntity>()))
                .Callback<TEntity>(entity => entities[entity.Guid] = entity);

            repoMock.Setup(x => x.FilterByAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), 
                    It.IsAny<Expression<Func<TEntity, It.IsAnyType>>>()))
                .ReturnsAsync((Expression<Func<TEntity, bool>> x, Expression<Func<TEntity, It.IsAnyType>> m) =>
                    entities.Values.Where(entity => x.Compile()(entity)).Select(entity => m.Compile()(entity)));
            return repoMock;
        }

    }
}
