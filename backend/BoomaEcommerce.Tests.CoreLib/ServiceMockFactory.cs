using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.Products;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Settings;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using Castle.Core.Logging;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace BoomaEcommerce.Tests.CoreLib
{
    public class ServiceMockFactory
    {
        public const string Secret = "aaaaaaaaaaaaaaaaaaa";

        private IDictionary<Guid, Store> _stores = new ConcurrentDictionary<Guid, Store>();
        private IDictionary<Guid, StoreOwnership> _storeOwnerships = new ConcurrentDictionary<Guid, StoreOwnership>();
        private IDictionary<Guid, StorePurchase> _storePurchases = new ConcurrentDictionary<Guid, StorePurchase>();
        private IDictionary<Guid, StoreManagement> _storeManagements = new ConcurrentDictionary<Guid, StoreManagement>();
        private IDictionary<Guid, StoreManagementPermission> _storeManagementPermissions = new ConcurrentDictionary<Guid, StoreManagementPermission>();
        private IDictionary<Guid, Product> _products = new ConcurrentDictionary<Guid, Product>();
        private IDictionary<Guid, Purchase> _purchases = new ConcurrentDictionary<Guid, Purchase>();
        private IDictionary<Guid, PurchaseProduct> _purchaseProducts = new ConcurrentDictionary<Guid, PurchaseProduct>();
        private IDictionary<Guid, ShoppingBasket> _shoppingBaskets = new ConcurrentDictionary<Guid, ShoppingBasket>();
        private IDictionary<Guid, ShoppingCart> _shoppingCarts = new ConcurrentDictionary<Guid, ShoppingCart>();
        private IDictionary<Guid, RefreshToken> _refreshTokens = new Dictionary<Guid, RefreshToken>();
        private IDictionary<Guid, User> _users = new Dictionary<Guid, User>();
        private Mock<UserManager<User>>  _userManagerMock = DalMockFactory.MockUserManager(new List<User>());

        public IStoresService MockStoreService()
        {
            var storeUnitOfWorkMock = DalMockFactory.MockStoreUnitOfWork(_stores, _storeOwnerships, _storePurchases,
                _storeManagements, _storeManagementPermissions, _products);
            var loggerMock = new Mock<ILogger<StoresService>>();
            return new StoresService(loggerMock.Object, MapperFactory.GetMapper(), storeUnitOfWorkMock.Object);
        }
        
        public IAuthenticationService MockAuthenticationService()
        {
            var loggerMock = new Mock<ILogger<AuthenticationService>>();
            var refreshTokenRepo = DalMockFactory.MockRepository(_refreshTokens);
            return new AuthenticationService(loggerMock.Object, _userManagerMock.Object,
                    new JwtSettings
                    {
                        Secret = Secret, TokenLifeTime = TimeSpan.FromHours(1), RefreshTokenExpirationMonthsAmount = 6

                    }, null, refreshTokenRepo.Object, MapperFactory.GetMapper());
        }
        
        public IPurchasesService MockPurchaseService()
        {
            var purchasesUnitOfWork = DalMockFactory
                .MockPurchasesUnitOfWork(_purchases, _products, _users, _shoppingCarts, _userManagerMock);
            var loggerMock = new Mock<ILogger<PurchasesService>>();
            var paymentClientMock = new Mock<IPaymentClient>();
            paymentClientMock.Setup(x => 
                x.MakeOrder(It.IsAny<Purchase>())).Returns(Task.CompletedTask);
            var supplyClientMock = new Mock<ISupplyClient>();
            supplyClientMock.Setup(x => 
                x.NotifyOrder(It.IsAny<Purchase>())).Returns(Task.CompletedTask);
            return new PurchasesService(MapperFactory.GetMapper(), loggerMock.Object, paymentClientMock.Object,
                purchasesUnitOfWork.Object, supplyClientMock.Object);
        }

        public IProductsService MockProductService()
        {
            var loggerMock = new Mock<ILogger<ProductsService>>();
            var productRepoMock = DalMockFactory.MockRepository(_products);
            var mistakeCorrectionMock = new Mock<IMistakeCorrection>();
            mistakeCorrectionMock.Setup(x => x.CorrectMistakeIfThereIsAny(It.IsAny<string>()))
                .Returns<string>((textToCorrect) => textToCorrect);
            return new ProductsService(loggerMock.Object, MapperFactory.GetMapper(),
                productRepoMock.Object, mistakeCorrectionMock.Object);
        }

        public IUsersService MockUserService()
        {
            var loggerMock = new Mock<ILogger<UsersService>>();
            var userUnitOfWork =
                DalMockFactory.MockUserUnitOfWork(_shoppingBaskets, _shoppingCarts, _purchaseProducts);
            return new UsersService(MapperFactory.GetMapper(), loggerMock.Object, userUnitOfWork.Object);
        }
    }
}