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
using ILogger = Castle.Core.Logging.ILogger;

namespace BoomaEcommerce.Services.Tests
{
    public class PurchaseServiceTests
    {

        private readonly Mock<ILogger<PurchasesService>> _loggerMock = new();
        private readonly Mock<IPaymentClient> _paymentClientMock = new();
        private readonly IFixture _fixture = new Fixture();

        public PurchaseServiceTests()
        {
            
        }

        private IMapper getMapper()
        {
            var mapperConfig = new MapperConfiguration(x =>
            {
                x.AddProfile(new DtoToDomainProfile());
                x.AddProfile(new DomainToDtoProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            return mapper;
        }

        private Mock<IPurchaseUnitOfWork> SetUpRepositoriesMocks(IDictionary<Guid, Purchase> purchases, IDictionary<Guid, Product> products,
            IDictionary<Guid, User> users)
        {
            var purchaseRepoMock = DalMockFactory.MockRepository(purchases);
            var productRepoMock = DalMockFactory.MockRepository(products);
            var userRepoMock = DalMockFactory.MockRepository(users);
            var purchaseUnitOfWork = new Mock<IPurchaseUnitOfWork>();
            purchaseUnitOfWork.SetupGet(x => x.PurchaseRepository).Returns(purchaseRepoMock.Object);
            purchaseUnitOfWork.SetupGet(x => x.ProductRepository).Returns(productRepoMock.Object);
            purchaseUnitOfWork.SetupGet(x => x.UserRepository).Returns(userRepoMock.Object);
            return purchaseUnitOfWork;
        }
        
        private Product GetTestProduct(Guid guid)
        {
            return _fixture.Build<Product>()
                .With(x => x.Guid, guid)
                .With(x => x.Amount, 10)
                .With(x => x.Price, 10)
                .With(x => x.IsSoftDeleted, false)
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
            var purchasesDict = new Dictionary<Guid, Purchase>();
            var productDict = new Dictionary<Guid, Product>();
            var userDict = new Dictionary<Guid, User>();
            
            var purchaseDtoFixture = _fixture.Build<PurchaseDto>()
                .With(x => x.StorePurchases, GetTestValidStorePurchasesDtos())
                .Create();
            
            var userFixture = _fixture.Build<User>()
                .With(x => x.Guid, purchaseDtoFixture.Buyer.Guid)
                .Create();
            userDict[purchaseDtoFixture.Buyer.Guid] = userFixture;
            
            foreach (var storePurchaseDto in purchaseDtoFixture.StorePurchases)
            {
                foreach (var productsPurchaseDto in storePurchaseDto.ProductsPurchases)
                {
                    var testProductGuid = productsPurchaseDto.ProductDto.Guid;
                    var testProduct = GetTestProduct(testProductGuid);
                    productDict[testProductGuid] = testProduct;
                }
            }

            var purchaseUnitOfWorkMock = SetUpRepositoriesMocks(purchasesDict, productDict, userDict);
        
            var sut = new PurchasesService(getMapper(), _loggerMock.Object,
                _paymentClientMock.Object, purchaseUnitOfWorkMock.Object);

            await sut.CreatePurchaseAsync(purchaseDtoFixture);

            foreach (var productDictValue in productDict.Values)
            {
                productDictValue.Amount.Should().Be(5);
            }

            purchasesDict[purchaseDtoFixture.Guid].Guid.Should().Be(purchaseDtoFixture.Guid);
        }


    }
}