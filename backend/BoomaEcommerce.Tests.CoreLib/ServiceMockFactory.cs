using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.Settings;
using BoomaEcommerce.Services.Stores;
using Castle.Core.Logging;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Moq;

namespace BoomaEcommerce.Tests.CoreLib
{
    public static class ServiceMockFactory
    {
        public const string Secret = "aaaaaaaaaaaaaaaaaaa";
        public static IStoresService MockStoreService()
        {
            var stores = new ConcurrentDictionary<Guid, Store>();
            var ownerships = new ConcurrentDictionary<Guid, StoreOwnership>();
            var purchases = new ConcurrentDictionary<Guid, StorePurchase>();
            var managements = new ConcurrentDictionary<Guid, StoreManagement>();
            var managementsPermissions = new ConcurrentDictionary<Guid, StoreManagementPermission>();
            var products = new ConcurrentDictionary<Guid, Product>();
            
            var storeUnitOfWorkMock = DalMockFactory.MockStoreUnitOfWork(stores, ownerships, purchases,
                managements, managementsPermissions, products);
            var loggerMock = new Mock<ILogger<StoresService>>();
            return new StoresService(loggerMock.Object, MapperFactory.GetMapper(), storeUnitOfWorkMock.Object);
        }
        
        public static IAuthenticationService MockAuthenticationService()
        {
            var userStore = new List<User>();
            var mockUserManager = DalMockFactory.MockUserManager(userStore);
            var refreshTokens = new Dictionary<Guid, RefreshToken>();
            var loggerMock = new Mock<ILogger<AuthenticationService>>();
            var refreshTokenRepo = DalMockFactory.MockRepository(refreshTokens);
            return new AuthenticationService(loggerMock.Object, mockUserManager.Object,
                    new JwtSettings
                    {
                        Secret = Secret, TokenLifeTime = TimeSpan.FromHours(1), RefreshTokenExpirationMonthsAmount = 6

                    }, null, refreshTokenRepo.Object, MapperFactory.GetMapper());
        }
    }
}