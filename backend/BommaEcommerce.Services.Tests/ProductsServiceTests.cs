using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BommaEcommerce.Services.Tests;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.MappingProfiles;
using BoomaEcommerce.Services.Products;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{

    public class ProductsServiceTests
    {
        Dictionary<Guid, Product> _EntitiesProducts = new Dictionary<Guid, Product>();

        [Fact]
        public async void GetProductTest()
        {
            var loggerMock = new Mock<ILogger<ProductsService>>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainToDtoProfile());
                cfg.AddProfile(new DtoToDomainProfile());
            });
            var mapper = config.CreateMapper();

            Store s1 = new() { };
            Store s2 = new() { };
            Product p1 = new() { Store = s1 };
            Product p2 = new() { Store = s1 };
            Product p3 = new() { Store = s2 };

            _EntitiesProducts.Add(p1.Guid, p1);
            _EntitiesProducts.Add(p2.Guid, p2);
            _EntitiesProducts.Add(p3.Guid, p3);

            var productsRepo = DalMockFactory.MockRepository(_EntitiesProducts);

            var productService = new ProductsService(loggerMock.Object, mapper, productsRepo.Object);

            var res =await productService.GetProductsFromStoreAsync(s1.Guid);

            List<ProductDto> expectedRes = new List<ProductDto>();
            expectedRes.Add(mapper.Map<ProductDto>(p1));
            expectedRes.Add(mapper.Map<ProductDto>(p2));


            res.ToList().Should().BeEquivalentTo(expectedRes);


        }
    }
}