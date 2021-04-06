using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.MappingProfiles;
using BoomaEcommerce.Services.Purchases;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class PurchaseServiceTests
    {
        private readonly PurchasesService _sut;
        private readonly Mock<ILogger<PurchasesService>> _loggerMock = new();
        private readonly Mock<IPaymentClient> _paymentClientMock = new();
        private readonly Mock<IRepository<User>> _userRepositoryMock = new();
        private readonly Mock<IRepository<Product>> _productRepositoryMock = new();
        private readonly Mock<IRepository<Purchase>> _purchaseRepositoryMock = new();
        private readonly IFixture _fixture = new Fixture();

        public PurchaseServiceTests()
        {
            var mapperConfig = new MapperConfiguration(x =>
            {
                x.AddProfile(new DtoToDomainProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            
            _sut = new PurchasesService(mapper, _loggerMock.Object, _paymentClientMock.Object,
                _userRepositoryMock.Object, _productRepositoryMock.Object, _purchaseRepositoryMock.Object);
        }
        
        private Product GetTestProduct(Guid guid)
        {
            return _fixture.Build<Product>()
                .With(x => x.Guid, guid)
                .With(x => x.Amount, 10)
                .With(x => x.Price, 10)
                .Create();
        }


        private List<PurchaseProductDto> GetTestValidPurchaseProductsDtos()
        {
            var validProductsPurchasesDtos = new List<PurchaseProductDto>
            {
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 5
                },
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 5
                },
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 5
                }
            };
            return validProductsPurchasesDtos;
        }

        private List<StorePurchaseDto> GetTestValidStorePurchasesDtos()
        {
            var validProductsPurchasesDtos = new List<StorePurchaseDto>
            {
                new() {ProductsPurchases = GetTestValidPurchaseProductsDtos()},
                new() {ProductsPurchases = GetTestValidPurchaseProductsDtos()},
                new() {ProductsPurchases = GetTestValidPurchaseProductsDtos()}
            };
            return validProductsPurchasesDtos;
        }

        [Fact]
        public async Task CreatePurchaseAsync_ShouldDecreaseProductsAmount_WhenPurchaseDtoIsValid()
        {
            var purchaseDtoFixture = _fixture.Build<PurchaseDto>()
                .With(x => x.StorePurchases, GetTestValidStorePurchasesDtos())
                .Create();
            _userRepositoryMock.Setup(x => x.FindByIdAsync(purchaseDtoFixture.Buyer.Guid))
                .ReturnsAsync(_fixture.Build<User>()
                    .With(x => x.Guid, purchaseDtoFixture.Buyer.Guid)
                    .Create());
            var testProducts = new List<Product>();
            foreach (var storePurchaseDto in purchaseDtoFixture.StorePurchases)
            {
                foreach (var productsPurchaseDto in storePurchaseDto.ProductsPurchases)
                {
                    var testProductGuid = productsPurchaseDto.ProductDto.Guid;
                    var testProduct = GetTestProduct(testProductGuid);
                    _productRepositoryMock.Setup(x => x.FindByIdAsync(testProductGuid))
                        .ReturnsAsync(testProduct);
                    testProducts.Add(testProduct);
                }
            }
            
            await _sut.CreatePurchaseAsync(purchaseDtoFixture);

            testProducts.ForEach(x => x.Amount.Should().Be(5));
            _userRepositoryMock.Invocations.Count.Should().Be(1);
            var productAmount = purchaseDtoFixture.StorePurchases.Aggregate(0,
                (count, storePurchase) => count + storePurchase.ProductsPurchases.Count);
            _productRepositoryMock.VerifyAll();
            _productRepositoryMock.Invocations.Count.Should().Be(productAmount);
            _purchaseRepositoryMock.Invocations.Count.Should().Be(1);

        }


    }
}