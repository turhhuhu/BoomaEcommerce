using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BommaEcommerce.Services.Tests;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.MappingProfiles;
using BoomaEcommerce.Services.Products;
using BoomaEcommerce.Services.Stores;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{

    public class ProductsServiceTests
    {
        Dictionary<Guid, Product> _EntitiesProducts = new Dictionary<Guid, Product>();
        Dictionary<Guid, Store> _EntitiesStores = new Dictionary<Guid, Store>();
        Dictionary<Guid, StorePurchase> _EntitiesStorePurchases = new Dictionary<Guid, StorePurchase>();

        static MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DomainToDtoProfile());
            cfg.AddProfile(new DtoToDomainProfile());
        });
        IMapper mapper = config.CreateMapper();

        [Fact]
        public async void GetProductTest()
        {
            Mock<ILogger<ProductsService>> loggerMock = new Mock<ILogger<ProductsService>>();
            Store s1 = new() { };
            Store s2 = new() { };
            Store s3 = new() { };
            Product p1 = new() { Store = s1 };
            Product p2 = new() { Store = s1 };
            Product p3 = new() { Store = s2 };

            _EntitiesProducts.Add(p1.Guid, p1);
            _EntitiesProducts.Add(p2.Guid, p2);
            _EntitiesProducts.Add(p3.Guid, p3);

            var productsRepo = DalMockFactory.MockRepository(_EntitiesProducts);

            var productService = new ProductsService(loggerMock.Object, mapper, productsRepo.Object);

            var res1 =await productService.GetProductsFromStoreAsync(s1.Guid);
            List<ProductDto> expectedRes1 = new List<ProductDto>();
            expectedRes1.Add(mapper.Map<ProductDto>(p1));
            expectedRes1.Add(mapper.Map<ProductDto>(p2));

            var res2 = await productService.GetProductsFromStoreAsync(s3.Guid);


            res1.ToList().Should().BeEquivalentTo(expectedRes1);
            res2.Should().BeEmpty();

        }

        [Fact]
        public async void GetStoreTest()
        {
            Mock<ILogger<StoresService>> loggerMock = new Mock<ILogger<StoresService>>();

            Store s1 = new() {StoreName="Benny Hadayag"};
            Store s2 = new() {StoreName="Nike"};
            Store s3 = new() {StoreName="Adidas"};
            Store s4 = new() { StoreName = "TopShop" };

            _EntitiesStores.Add(s1.Guid, s1);
            _EntitiesStores.Add(s2.Guid, s2);
            _EntitiesStores.Add(s3.Guid, s3);

            var storesRepo = DalMockFactory.MockRepository(_EntitiesStores);
            var storesPurchaseRepo = DalMockFactory.MockRepository(_EntitiesStorePurchases);
            var storesService = new StoresService(loggerMock.Object, mapper,storesRepo.Object,storesPurchaseRepo.Object);

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


    }
}