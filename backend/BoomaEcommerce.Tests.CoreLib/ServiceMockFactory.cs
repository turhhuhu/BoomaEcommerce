using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies;
using BoomaEcommerce.Domain.ProductOffer;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Discounts;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.External.Payment;
using BoomaEcommerce.Services.External.Supply;
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
        public const string Secret = "aaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        private IDictionary<Guid, Store> _stores = new ConcurrentDictionary<Guid, Store>();
        private IDictionary<Guid, StoreOwnership> _storeOwnerships = new ConcurrentDictionary<Guid, StoreOwnership>();
        private IDictionary<Guid, StorePurchase> _storePurchases = new ConcurrentDictionary<Guid, StorePurchase>();
        private IDictionary<Guid, StoreManagement> _storeManagements = new ConcurrentDictionary<Guid, StoreManagement>();
        private IDictionary<Guid, StoreManagementPermissions> _storeManagementPermissions = new ConcurrentDictionary<Guid, StoreManagementPermissions>();
        private IDictionary<Guid, Product> _products = new ConcurrentDictionary<Guid, Product>();
        private IDictionary<Guid, Purchase> _purchases = new ConcurrentDictionary<Guid, Purchase>();
        private IDictionary<Guid, PurchaseProduct> _purchaseProducts = new ConcurrentDictionary<Guid, PurchaseProduct>();
        private IDictionary<Guid, ShoppingBasket> _shoppingBaskets = new ConcurrentDictionary<Guid, ShoppingBasket>();
        private IDictionary<Guid, ShoppingCart> _shoppingCarts = new ConcurrentDictionary<Guid, ShoppingCart>();
        private IDictionary<Guid, RefreshToken> _refreshTokens = new Dictionary<Guid, RefreshToken>();
        private IDictionary<Guid, User> _users = new Dictionary<Guid, User>();
        private IDictionary<Guid, Notification> _notifications = new ConcurrentDictionary<Guid, Notification>();
        private IDictionary<Guid, Policy> _policies = new ConcurrentDictionary<Guid, Policy>();
        private IDictionary<Guid, Discount> _discounts = new ConcurrentDictionary<Guid, Discount>();
        private IDictionary<Guid, ProductOffer> _productOffers = new ConcurrentDictionary<Guid, ProductOffer>();
        private IDictionary<Guid, ApproverOwner> _approverOwners = new ConcurrentDictionary<Guid, ApproverOwner>();
        private Mock<UserManager<User>> _userManagerMock;
        private INotificationPublisher _notificationPublisherStub = new NotificationPublisherStub();

        public ServiceMockFactory()
        {
            _userManagerMock = DalMockFactory.MockUserManager(_users);
        }
        public IStoresService MockStoreService()
        {
            var storeUnitOfWorkMock = DalMockFactory.MockStoreUnitOfWork(_stores, _storeOwnerships, _storePurchases,
                _storeManagements, _storeManagementPermissions, _products,_policies, _discounts, _users, _productOffers, _approverOwners );
            var loggerMock = new Mock<ILogger<StoresService>>();
            return new StoresService(loggerMock.Object, MapperFactory.GetMapper(), storeUnitOfWorkMock.Object, new NotificationPublisherStub());
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
                .MockPurchasesUnitOfWork(_purchases, _products, _users, _shoppingCarts, _storeOwnerships, _notifications, _stores, _productOffers, _storePurchases, _purchaseProducts);

            var loggerMock = new Mock<ILogger<PurchasesService>>();

            var paymentClientMock = new Mock<IPaymentClient>();

            paymentClientMock.Setup(x => 
                x.MakePayment(It.IsAny<PaymentDetailsDto>())).Returns(Task.FromResult(10001));

            var supplyClientMock = new Mock<ISupplyClient>();

            supplyClientMock.Setup(x => 
                x.MakeOrder(It.IsAny<SupplyDetailsDto>())).Returns(Task.FromResult(10001));

            return new PurchasesService(MapperFactory.GetMapper(), loggerMock.Object, paymentClientMock.Object,
                purchasesUnitOfWork.Object, supplyClientMock.Object, _notificationPublisherStub);
        }

        public NotificationPublisherStub GetNotificationPublisherStub()
        {
            return (NotificationPublisherStub) _notificationPublisherStub;
        }

        public IPurchasesService MockPurchaseServiceWithCollapsingExternalSystem()
        {
            var purchasesUnitOfWork = DalMockFactory
                .MockPurchasesUnitOfWork(_purchases, _products, _users, _shoppingCarts, _storeOwnerships, _notifications, _stores, _productOffers, _storePurchases, _purchaseProducts);

            var loggerMock = new Mock<ILogger<PurchasesService>>();

            var paymentClientMock = new Mock<IPaymentClient>();

            paymentClientMock.Setup(x =>
                x.MakePayment(It.IsAny<PaymentDetailsDto>())).Returns(Task.FromResult(10001));

            var supplyClientMock = new Mock<ISupplyClient>();

            supplyClientMock.Setup(x =>
                x.MakeOrder(It.IsAny<SupplyDetailsDto>())).Throws(new Exception("Supply system collapsed"));

            return new PurchasesService(MapperFactory.GetMapper(), loggerMock.Object, paymentClientMock.Object,
                purchasesUnitOfWork.Object, supplyClientMock.Object, Mock.Of<INotificationPublisher>());
        }

        public IPurchasesService MockPurchaseServiceWithCollapsingSupplyExternalSystem()
        {
            var purchasesUnitOfWork = DalMockFactory
                .MockPurchasesUnitOfWork(_purchases, _products, _users, _shoppingCarts, _storeOwnerships, _notifications, _stores, _productOffers, _storePurchases, _purchaseProducts);

            var loggerMock = new Mock<ILogger<PurchasesService>>();

            var paymentClientMock = new Mock<IPaymentClient>();

            paymentClientMock.Setup(x =>
                x.MakePayment(It.IsAny<PaymentDetailsDto>())).Throws(new Exception("External system collapsed"));

            var supplyClientMock = new Mock<ISupplyClient>();

            supplyClientMock.Setup(x =>
                x.MakeOrder(It.IsAny<SupplyDetailsDto>())).Returns(Task.FromResult(10001));

            return new PurchasesService(MapperFactory.GetMapper(), loggerMock.Object, paymentClientMock.Object,
                purchasesUnitOfWork.Object, supplyClientMock.Object, Mock.Of<INotificationPublisher>());
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
                DalMockFactory.MockUserUnitOfWork(_shoppingBaskets, _shoppingCarts, _products, _productOffers, _storeOwnerships, _users);
            return new UsersService(MapperFactory.GetMapper(), loggerMock.Object, userUnitOfWork.Object, _notificationPublisherStub);
        }
    }
}