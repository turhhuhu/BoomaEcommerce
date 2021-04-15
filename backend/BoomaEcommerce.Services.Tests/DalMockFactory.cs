using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BoomaEcommerce.Core;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace BoomaEcommerce.Services.Tests
{
    public static class DalMockFactory
    {
        public static Mock<UserManager<User>> MockUserManager(List<User> ls)
        {
            if (ls == null)
            {
                throw new NullReferenceException("User or user list must be provided to mock the UserManager.");
            }
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            mgr.Object.UserValidators.Add(new UserValidator<User>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<User>());
            
            mgr.Setup(userManger => userManger.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string guid) => ls.FirstOrDefault(x => x.Guid.ToString().Equals(guid)));

            mgr.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            mgr.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<User, string>((x, y) => ls.Add(x));

            mgr.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            mgr.Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            mgr.Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string username) => ls.FirstOrDefault(usr => usr.UserName == username));

            mgr.Setup(userManager => userManager.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        public static Mock<IRepository<TEntity>> MockRepository<TEntity>(IDictionary<Guid, TEntity> entities)
            where TEntity : BaseEntity
        {
            if (entities == null)
            {
                return null;
            }
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
                    (Expression<Func<TEntity, It.IsAnyType>>)It.IsAny<object>()))
                .ReturnsAsync((Expression<Func<TEntity, bool>> x, Expression<Func<TEntity, It.IsAnyType>> m) =>
                    entities.Values.Where(entity => x.Compile()(entity)).Select(entity => m.Compile()(entity)));

            repoMock.Setup(x => x.FindAllAsync())
                .ReturnsAsync(entities.Values);

            return repoMock;
        }
        public static Mock<IStoreUnitOfWork> MockStoreUnitOfWork(
            IDictionary<Guid, Store> stores,
            IDictionary<Guid, StoreOwnership> storeOwnerships,
            IDictionary<Guid, StorePurchase> storePurchases,
            IDictionary<Guid, StoreManagement> storeManagements,
            IDictionary<Guid, StoreManagementPermission> storeManagementPermissions
        )
        {

            var storeRepoMock = MockRepository(stores);
            var storeOwnershipRepoMock = MockRepository(storeOwnerships);
            var storePurchasesRepoMock = MockRepository(storePurchases);
            var storeManagementRepoMock = MockRepository(storeManagements);
            var storeManagementPermissionsRepoMock = MockRepository(storeManagementPermissions);

            var storeUnitOfWorkMock = new Mock<IStoreUnitOfWork>();
            storeUnitOfWorkMock.SetupGet(x => x.StoreRepo).Returns(storeRepoMock?.Object);
            storeUnitOfWorkMock.SetupGet(x => x.StoreOwnershipRepo).Returns(storeOwnershipRepoMock?.Object);
            storeUnitOfWorkMock.SetupGet(x => x.StorePurchaseRepo).Returns(storePurchasesRepoMock?.Object);
            storeUnitOfWorkMock.SetupGet(x => x.StoreManagementRepo).Returns(storeManagementRepoMock?.Object);
            storeUnitOfWorkMock.SetupGet(x => x.StoreManagementPermissionsRepo).Returns(storeManagementPermissionsRepoMock?.Object);
            return storeUnitOfWorkMock;
        }


        public static Mock<IPurchaseUnitOfWork> MockPurchasesUnitOfWork(
            IDictionary<Guid, Purchase> purchases,
            IDictionary<Guid, Product> products,
            IDictionary<Guid, User> users,
            IDictionary<Guid, ShoppingCart> shoppingCarts)
        {
            var purchaseRepoMock = MockRepository(purchases);
            var productRepoMock = MockRepository(products);
            var userRepoMock = MockUserManager(users.Values.ToList());
            var shoppingCartMock = MockRepository(shoppingCarts);
            var purchaseUnitOfWork = new Mock<IPurchaseUnitOfWork>();
            purchaseUnitOfWork.SetupGet(x => x.PurchaseRepository).Returns(purchaseRepoMock?.Object);
            purchaseUnitOfWork.SetupGet(x => x.ProductRepository).Returns(productRepoMock?.Object);
            purchaseUnitOfWork.SetupGet(x => x.UserRepository).Returns(userRepoMock?.Object);
            purchaseUnitOfWork.SetupGet(x => x.ShoppingCartRepository).Returns(shoppingCartMock?.Object);
            return purchaseUnitOfWork;
        }
        
        public static Mock<IUserUnitOfWork> MockUserUnitOfWork(
            IDictionary<Guid, ShoppingBasket> shoppingBaskets,
            IDictionary<Guid, ShoppingCart> shoppingCarts, 
            IDictionary<Guid, PurchaseProduct> purchaseProducts)
        {
            var shoppingBasketRepoMock = DalMockFactory.MockRepository(shoppingBaskets);
            var shoppingCartRepoMock = DalMockFactory.MockRepository(shoppingCarts);
            var purchaseProductRepoMock = DalMockFactory.MockRepository(purchaseProducts);

            var userUnitOfWork = new Mock<IUserUnitOfWork>();
            userUnitOfWork.SetupGet(x => x.ShoppingBasketRepo).Returns(shoppingBasketRepoMock?.Object);
            userUnitOfWork.SetupGet(x => x.ShoppingCartRepo).Returns(shoppingCartRepoMock?.Object);
            userUnitOfWork.SetupGet(x => x.PurchaseProductRepo).Returns(purchaseProductRepoMock?.Object);
            return userUnitOfWork;
        }
    }
}
