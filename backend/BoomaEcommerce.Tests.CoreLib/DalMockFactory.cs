using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BoomaEcommerce.Core;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Policies;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace BoomaEcommerce.Tests.CoreLib
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

            var rolesStore = new Dictionary<Guid, HashSet<string>>();
            var passwordStore = new Dictionary<Guid, string>();

            mgr.Object.UserValidators.Add(new UserValidator<User>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<User>());
            
            mgr.Setup(userManger => userManger.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string guid) => ls.FirstOrDefault(x => x.Guid.ToString().Equals(guid)));

            mgr.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            mgr.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<User, string>((x, y) =>
                {
                    ls.Add(x);
                    passwordStore.Add(x.Guid, y);
                });

            mgr.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            mgr.Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync((User user, string password) => passwordStore[user.Guid].Equals(password));

            mgr.Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string username) => ls.FirstOrDefault(usr => usr.UserName == username));

            mgr.Setup(userManager => userManager.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<User, string>((user, role) =>
                {
                    if (!rolesStore.ContainsKey(user.Guid))
                    {
                        rolesStore.Add(user.Guid, new HashSet<string>());
                    }
                    rolesStore[user.Guid].Add(role);
                });

            mgr.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) => rolesStore.ContainsKey(user.Guid) 
                    ? rolesStore[user.Guid].ToList() 
                    : new List<string>());

            return mgr;
        }

        public static Mock<IRepository<TEntity>> MockRepository<TEntity>(IDictionary<Guid, TEntity> entities)
            where TEntity : class, IBaseEntity
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
                .Callback<IEnumerable<TEntity>>(entitiesToAdd =>
                    entitiesToAdd.ToList().ForEach(ent => entities.Add(ent.Guid, ent)));

            repoMock.Setup(x => x.ReplaceOneAsync(It.IsAny<TEntity>()))
                .Callback<TEntity>(entity => entities[entity.Guid] = entity);

            repoMock.Setup(x => x.FilterByAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), 
                    (Expression<Func<TEntity, It.IsAnyType>>)It.IsAny<object>()))
                .ReturnsAsync((Expression<Func<TEntity, bool>> x, Expression<Func<TEntity, It.IsAnyType>> m) =>
                    entities.Values.Where(entity => x.Compile()(entity)).Select(entity => m.Compile()(entity)));

            repoMock.Setup(x => x.FindAllAsync())
                .ReturnsAsync(() => entities.Values);

            return repoMock;
        }

        public static Mock<IStoreUnitOfWork> MockStoreUnitOfWork(
            IDictionary<Guid, Store> stores,
            IDictionary<Guid, StoreOwnership> storeOwnerships,
            IDictionary<Guid, StorePurchase> storePurchases,
            IDictionary<Guid, StoreManagement> storeManagements,
            IDictionary<Guid, StoreManagementPermissions> storeManagementPermissions,
            IDictionary<Guid, Product> products,
            IDictionary<Guid,Policy> policies,
            IDictionary<Guid, User> users
        )
        {
            var userRepoMock = MockRepository(users);
            var storeRepoMock = MockRepository(stores);
            var storeOwnershipRepoMock = MockRepository(storeOwnerships);
            var storePurchasesRepoMock = MockRepository(storePurchases);
            var storeManagementRepoMock = MockRepository(storeManagements);
            var storePolicyRepoMock = MockRepository(policies);


            // Mock do to delete on cascade 
            storeOwnershipRepoMock?.Setup(x => x.DeleteByIdAsync(It.IsAny<Guid>()))
                .Callback<Guid>(soGuid =>
                {
                    var storeOwner = storeOwnerships[soGuid];

                    var subOrdinates = storeOwner.GetSubordinates();

                    foreach (var sm in subOrdinates.Item2)
                    {
                        storeManagements.Remove(sm.Guid);
                    }

                    foreach (var so in subOrdinates.Item1)
                    {
                        storeOwnerships.Remove(so.Guid);
                    }

                    storeOwnerships.Remove(soGuid);
                });

            storeManagementRepoMock?.Setup(x => x.InsertOneAsync(It.IsAny<StoreManagement>()))
                .Callback<StoreManagement>(sm =>
                {
                    storeManagements.Add(sm.Guid, sm);
                    storeManagementPermissions?.Add(sm.Permissions.Guid, sm.Permissions);
                });

            var storeManagementPermissionsRepoMock = MockRepository(storeManagementPermissions);

            storeManagementPermissionsRepoMock?.Setup(x => x.ReplaceOneAsync(It.IsAny<StoreManagementPermissions>()))
                .Callback<StoreManagementPermissions>(smp =>
                {
                    storeManagementPermissions[smp.Guid] = smp;
                    if (storeManagements != null && storeManagements.TryGetValue(smp.Guid, out var storeManagement))
                    {
                        storeManagement.Permissions = smp;
                    }
                });

            var productsRepoMock = MockRepository(products);
            var storeUnitOfWorkMock = new Mock<IStoreUnitOfWork>();
            storeUnitOfWorkMock.SetupGet(x => x.StoreRepo).Returns(storeRepoMock?.Object);
            storeUnitOfWorkMock.SetupGet(x => x.StoreOwnershipRepo).Returns(storeOwnershipRepoMock?.Object);
            storeUnitOfWorkMock.SetupGet(x => x.StorePurchaseRepo).Returns(storePurchasesRepoMock?.Object);
            storeUnitOfWorkMock.SetupGet(x => x.StoreManagementRepo).Returns(storeManagementRepoMock?.Object);
            storeUnitOfWorkMock.SetupGet(x => x.StoreManagementPermissionsRepo).Returns(storeManagementPermissionsRepoMock?.Object);
            storeUnitOfWorkMock.SetupGet(x => x.ProductRepo).Returns(productsRepoMock?.Object);
            storeUnitOfWorkMock.SetupGet(x => x.PolicyRepo).Returns(storePolicyRepoMock?.Object);


            return storeUnitOfWorkMock;
        }


        public static Mock<IPurchaseUnitOfWork> MockPurchasesUnitOfWork(
            IDictionary<Guid, Purchase> purchases,
            IDictionary<Guid, Product> products,
            IDictionary<Guid, User> users,
            IDictionary<Guid, ShoppingCart> shoppingCarts,
            IDictionary<Guid, StoreOwnership> ownerships,
            IDictionary<Guid, Notification> notifications,
            IDictionary<Guid, Store> stores,
            IDictionary<Guid, StorePurchase> storePurchases = null,
            IDictionary<Guid, PurchaseProduct> purchaseProducts = null,
            Mock<UserManager<User>> userManagerMock = null)
        {
            var purchaseRepoMock = MockRepository(purchases);
            purchaseRepoMock?.Setup(x => x.InsertOneAsync(It.IsAny<Purchase>()))
                .Callback<Purchase>(purchase
                         =>
                {
                    purchases.Add(purchase.Guid, purchase);
                    foreach (var storePurchase in purchase
                        .StorePurchases)
                    {
                        storePurchases?.Add(storePurchase.Guid, storePurchase);
                        foreach (var purchaseProduct in storePurchase.PurchaseProducts)
                        {
                            purchaseProducts?.Add(purchaseProduct.Guid, purchaseProduct);
                            products[purchaseProduct.Product.Guid] = purchaseProduct.Product;
                        }
                    }
                });

            var storesRepoMock = MockRepository(stores);
            var productRepoMock = MockRepository(products);
            var userRepoMock = userManagerMock ?? MockUserManager(users is null ? new List<User>() : users.Values.ToList());
            var shoppingCartMock = MockRepository(shoppingCarts);
            var ownershipsMock = MockRepository(ownerships);
            var notificationsMock = MockRepository(notifications);
            var purchaseUnitOfWork = new Mock<IPurchaseUnitOfWork>();
            purchaseUnitOfWork.SetupGet(x => x.StoreOwnershipRepository).Returns(ownershipsMock?.Object);
            purchaseUnitOfWork.SetupGet(x => x.PurchaseRepository).Returns(purchaseRepoMock?.Object);
            purchaseUnitOfWork.SetupGet(x => x.ProductRepository).Returns(productRepoMock?.Object);
            purchaseUnitOfWork.SetupGet(x => x.UserRepository).Returns(userRepoMock?.Object);
            purchaseUnitOfWork.SetupGet(x => x.ShoppingCartRepository).Returns(shoppingCartMock?.Object);
            purchaseUnitOfWork.SetupGet(x => x.StoresRepository).Returns(storesRepoMock?.Object);
            return purchaseUnitOfWork;
        }
        
        public static Mock<IUserUnitOfWork> MockUserUnitOfWork(
            IDictionary<Guid, ShoppingBasket> shoppingBaskets,
            IDictionary<Guid, ShoppingCart> shoppingCarts, 
            Mock<UserManager<User>> userManagerMock = null)
        {
            var shoppingBasketRepoMock = DalMockFactory.MockRepository(shoppingBaskets);
            var shoppingCartRepoMock = DalMockFactory.MockRepository(shoppingCarts);

            var userUnitOfWork = new Mock<IUserUnitOfWork>();
            userUnitOfWork.SetupGet(x => x.ShoppingBasketRepo).Returns(shoppingBasketRepoMock?.Object);
            userUnitOfWork.SetupGet(x => x.ShoppingCartRepo).Returns(shoppingCartRepoMock?.Object);
            userUnitOfWork.SetupGet(x => x.UserManager).Returns(userManagerMock?.Object);
            return userUnitOfWork;
        }
    }
}
