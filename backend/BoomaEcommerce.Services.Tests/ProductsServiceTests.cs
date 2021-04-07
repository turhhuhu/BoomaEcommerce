using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Products;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class ProductsServiceTests
    {
        private readonly Mock<ILogger<ProductsService>> _logger = new();
        private readonly IMapper _mapper = MapperFactory.GetMapper();

        [Fact]
        public async Task GetProductsFromStoreAsync_ReturnsAllNotSoftDeletedProductsFromGivenStore_WhenStoreExists()
        {
            var storeGuid = Guid.NewGuid();
            var notSoftDeletedProductGuidList = new List<Guid>();
            var productsDict = new Dictionary<Guid, Product>();
            for (var i = 0; i < 3; i++)
            {
                var notSoftDeletedProductGuid = Guid.NewGuid();
                var notSoftDeletedProduct = TestData.GetTestProductFromStore(notSoftDeletedProductGuid, storeGuid);
                productsDict.Add(notSoftDeletedProductGuid, notSoftDeletedProduct);
                notSoftDeletedProductGuidList.Add(notSoftDeletedProductGuid);
            }
            var softDeletedProductGuid = Guid.NewGuid();
            productsDict.Add(softDeletedProductGuid,
                new Product{Guid = softDeletedProductGuid, Store = new Store{Guid =  storeGuid}, IsSoftDeleted = true});
            var productRepoMock = DalMockFactory.MockRepository(productsDict);

            var sut = new ProductsService(_logger.Object, _mapper, productRepoMock.Object);

            var result = await sut.GetProductsFromStoreAsync(storeGuid);

            foreach (var productDto in result)
            {
                productDto.Store.Guid.Should().Be(storeGuid);
                notSoftDeletedProductGuidList.Should().Contain(productDto.Guid);
            }
            
        }
        
        [Fact]
        public async Task GetProductsFromStoreAsync_ReturnsNull_WhenStoreDoesNotExists()
        {
            var productsDict = new Dictionary<Guid, Product>();
            var productRepoMock = DalMockFactory.MockRepository(productsDict);
            var sut = new ProductsService(_logger.Object, _mapper, productRepoMock.Object);

            var result = await sut.GetProductsFromStoreAsync(Guid.NewGuid());

            result.Should().BeEmpty();
        }
        
        [Fact]
        public async Task GetProductAsync_ReturnsNotSafeDeletedProduct_WhenProductExistsAndNotSafeDeleted()
        {
            var productsDict = new Dictionary<Guid, Product>();
            var productGuid = Guid.NewGuid();
            productsDict[productGuid] = TestData.GetTestProduct(productGuid);
            var productRepoMock = DalMockFactory.MockRepository(productsDict);
            var sut = new ProductsService(_logger.Object, _mapper, productRepoMock.Object);

            var result = await sut.GetProductAsync(productGuid);

            result.Guid.Should().Be(productGuid);
        }
        
        [Fact]
        public async Task GetProductAsync_ReturnsNull_WhenProductDoesNotExist()
        {
            var productsDict = new Dictionary<Guid, Product>();
            var productRepoMock = DalMockFactory.MockRepository(productsDict);
            var sut = new ProductsService(_logger.Object, _mapper, productRepoMock.Object);

            var result = await sut.GetProductAsync(Guid.NewGuid());

            result.Should().BeNull();
        }
        
        [Fact]
        public async Task GetProductAsync_ReturnsNull_WhenProductExistsButIsSafeDeleted()
        {
            var productsDict = new Dictionary<Guid, Product>();
            var productGuid = Guid.NewGuid();
            productsDict[productGuid] = new Product{Guid = productGuid, IsSoftDeleted = true};
            var productRepoMock = DalMockFactory.MockRepository(productsDict);
            var sut = new ProductsService(_logger.Object, _mapper, productRepoMock.Object);

            var result = await sut.GetProductAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteProductAsync_ReturnTrueAndProductIsSafeDeleted_WhenProductExistsAndIsNotSafeDeleted()
        {
            var productsDict = new Dictionary<Guid, Product>();
            var productGuid = Guid.NewGuid();
            productsDict[productGuid] = TestData.GetTestProduct(productGuid);
            var productRepoMock = DalMockFactory.MockRepository(productsDict);
            var sut = new ProductsService(_logger.Object, _mapper, productRepoMock.Object);

            var result = await sut.DeleteProductAsync(productGuid);

            result.Should().BeTrue();
            productsDict[productGuid].IsSoftDeleted.Should().BeTrue();
        }
        
        [Fact]
        public async Task DeleteProductAsync_ReturnFalse_WhenProductDoesntNotExist()
        {
            var productsDict = new Dictionary<Guid, Product>();
            var productRepoMock = DalMockFactory.MockRepository(productsDict);
            var sut = new ProductsService(_logger.Object, _mapper, productRepoMock.Object);

            var result = await sut.DeleteProductAsync(Guid.NewGuid());

            result.Should().BeFalse();
        }
        
        [Fact]
        public async Task DeleteProductAsync_ReturnFalse_WhenProductExistsAndIsSafeDeleted()
        {
            var productsDict = new Dictionary<Guid, Product>();
            var productGuid = Guid.NewGuid();
            productsDict[productGuid] = new Product{Guid = productGuid, IsSoftDeleted = true};
            var productRepoMock = DalMockFactory.MockRepository(productsDict);
            var sut = new ProductsService(_logger.Object, _mapper, productRepoMock.Object);

            var result = await sut.DeleteProductAsync(productGuid);

            result.Should().BeFalse();
            productsDict.Keys.Should().Contain(productGuid);
        }
        
        [Fact]
        public async Task UpdateProductAsync_ReturnsNotSafeDeletedProduct_WhenProductExistsAndNotSafeDeleted()
        {
            var productsDict = new Dictionary<Guid, Product>();
            var productToReplaceGuid = Guid.NewGuid();
            productsDict[productToReplaceGuid] = TestData.GetTestProduct(productToReplaceGuid);
            var productRepoMock = DalMockFactory.MockRepository(productsDict);
            var sut = new ProductsService(_logger.Object, _mapper, productRepoMock.Object);

            var replacementProductDto =
                new ProductDto
                    {Guid = productToReplaceGuid, Amount = 5, Price = 5, Name = "ChessBoard", Category = "Chess"}; 

            var result = await sut.UpdateProductAsync(replacementProductDto);

            result.Should().BeTrue();
            var resultProduct = productsDict[productToReplaceGuid];
            resultProduct.Amount.Should().Be(5);
            resultProduct.Price.Should().Be(5);
            resultProduct.Category.Should().Be("Chess");
            resultProduct.Name.Should().Be("ChessBoard");
        }
        
        [Fact]
        public async Task UpdateProductAsync_ReturnsFalse_WhenProductDoesNotExist()
        {
            var productsDict = new Dictionary<Guid, Product>();
            var productRepoMock = DalMockFactory.MockRepository(productsDict);
            var sut = new ProductsService(_logger.Object, _mapper, productRepoMock.Object);

            var result = await sut.UpdateProductAsync(new ProductDto{Guid = Guid.NewGuid()});

            result.Should().BeFalse();
        }
        
        [Fact]
        public async Task UpdateProductAsync_ReturnsFalse_WhenProductExistsButIsSafeDeleted()
        {
            var productsDict = new Dictionary<Guid, Product>();
            var productGuid = Guid.NewGuid();
            productsDict[productGuid] = new Product{Guid = productGuid, IsSoftDeleted = true};
            var productRepoMock = DalMockFactory.MockRepository(productsDict);
            var sut = new ProductsService(_logger.Object, _mapper, productRepoMock.Object);

            var result = await sut.UpdateProductAsync(new ProductDto{Guid = productGuid});

            result.Should().BeFalse();
        }
        
        
    }
}