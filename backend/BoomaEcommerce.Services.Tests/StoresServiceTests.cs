using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoMapper;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.MappingProfiles;
using BoomaEcommerce.Services.Stores;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class StoresServiceTests
    {
        Dictionary<Guid, Store> _EntitiesStores = new Dictionary<Guid, Store>();
        Dictionary<Guid, StorePurchase> _EntitiesStorePurchases = new Dictionary<Guid, StorePurchase>();
        static MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DomainToDtoProfile());
            cfg.AddProfile(new DtoToDomainProfile());
        });
        IMapper mapper = config.CreateMapper();
        private IFixture _fixture = new Fixture();
       
        [Fact]
        public async void GetStoreTest()
        {
            Mock<ILogger<StoresService>> loggerMock = new Mock<ILogger<StoresService>>();

            Store s1 = new() { StoreName = "Benny Hadayag" };
            Store s2 = new() { StoreName = "Nike" };
            Store s3 = new() { StoreName = "Adidas" };
            Store s4 = new() { StoreName = "TopShop" };

            _EntitiesStores.Add(s1.Guid, s1);
            _EntitiesStores.Add(s2.Guid, s2);
            _EntitiesStores.Add(s3.Guid, s3);

            var storesRepo = DalMockFactory.MockRepository(_EntitiesStores);
            var storesPurchaseRepo = DalMockFactory.MockRepository(_EntitiesStorePurchases);
            var storesService = new StoresService(loggerMock.Object, mapper, storesRepo.Object, storesPurchaseRepo.Object);

            var res1 = await storesService.GetStoreAsync(s1.Guid);
            var expectedRes1 = mapper.Map<StoreDto>(s1);
            res1.Should().BeEquivalentTo(expectedRes1);

            var res2 = await storesService.GetStoreAsync(s4.Guid);
            res2.Should().BeNull();

            var res3 = await storesService.GetStoresAsync();
            List<StoreDto> expectedRes3 = new List<StoreDto>();
            expectedRes3.Add(mapper.Map<StoreDto>(s1));
            expectedRes3.Add(mapper.Map<StoreDto>(s2));
            expectedRes3.Add(mapper.Map<StoreDto>(s3));
            res3.ToList().Should().BeEquivalentTo(expectedRes3);
        }

        [Fact]
        public async void GetStorePurchaseHistoryTest()
        {
            Mock<ILogger<StoresService>> loggerMock = new Mock<ILogger<StoresService>>();
            Store s1 = new() {};
            Store s2 = new() {};
            Store s3 = new() {};

            //store1 purchase 
                //p1 
            var pr1 = _fixture.Build<PurchaseProduct>().Create();
            var pr2 = _fixture.Build<PurchaseProduct>().Create();
            var pr3 = _fixture.Build<PurchaseProduct>().Create();

            var prList1 = new List<PurchaseProduct>();
            prList1.Add(pr1);
            prList1.Add(pr2);
            prList1.Add(pr3);

            StorePurchase p1 = new() {ProductsPurchases = prList1 , Store = s1};
               
                //p2
            var pr4 = _fixture.Build<PurchaseProduct>().Create();
            var pr5 = _fixture.Build<PurchaseProduct>().Create(); 

            var prList2 = new List<PurchaseProduct>();
            prList2.Add(pr4);
            prList2.Add(pr5);
            

            StorePurchase p2 = new() { ProductsPurchases = prList2, Store = s1 };

            //store2 purchase 
            var pr6 = _fixture.Build<PurchaseProduct>().Create();
            var prList3 = new List<PurchaseProduct>();
            prList3.Add(pr6);

            StorePurchase p3 = new() { ProductsPurchases = prList3, Store = s2 };

            _EntitiesStorePurchases.Add(p1.Guid, p1);
            _EntitiesStorePurchases.Add(p2.Guid, p2);
            _EntitiesStorePurchases.Add(p3.Guid, p3);

            _EntitiesStores.Add(s1.Guid,s1);
            _EntitiesStores.Add(s2.Guid, s2);

            var storesRepo = DalMockFactory.MockRepository(_EntitiesStores);
            var storesPurchaseRepo = DalMockFactory.MockRepository(_EntitiesStorePurchases);
            var storesService = new StoresService(loggerMock.Object, mapper, storesRepo.Object, storesPurchaseRepo.Object);

            var res1 = await storesService.GetStorePurchaseHistory(s1.Guid);
            var expectedRes1 = new List<StorePurchaseDto>();
            expectedRes1.Add(mapper.Map<StorePurchaseDto>(p1));
            expectedRes1.Add(mapper.Map<StorePurchaseDto>(p2));

            res1.ToList().Should().BeEquivalentTo(expectedRes1);

            var res2 = await storesService.GetStorePurchaseHistory(s2.Guid);
            var expectedRes2 = new List<StorePurchaseDto>();
            expectedRes2.Add(mapper.Map<StorePurchaseDto>(p3));

            res2.ToList().Should().BeEquivalentTo(expectedRes2);

            var res3 = await storesService.GetStorePurchaseHistory(s3.Guid);
            res3.Should().BeEmpty();


        }
    }
}